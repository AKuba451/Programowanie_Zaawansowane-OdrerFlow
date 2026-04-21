using System.Xml.Linq;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services;

public class XmlReportBuilder
{
    public XDocument BuildReport(IEnumerable<Order> orders)
    {
        var orderList = orders.ToList();
        
        XElement root = new XElement("report",
            new XAttribute("generated", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
            
            new XElement("summary",
                new XAttribute("totalOrders", orderList.Count),
                new XAttribute("totalRevenue", orderList.Sum(o => o.TotalAmount))
            ),
            
            new XElement("byStatus",
                orderList.GroupBy(o => o.Status)
                    .Select(g => new XElement("status",
                        new XAttribute("name", g.Key),
                        new XAttribute("count", g.Count()),
                        new XAttribute("revenue", g.Sum(x => x.TotalAmount))
                    ))
            ),
            
            new XElement("byCustomer",
                orderList.GroupBy(o => o.Customer)
                    .Select(cg => new XElement("customer",
                        new XAttribute("id", cg.Key.ID),
                        new XAttribute("name", cg.Key.Name),
                        new XAttribute("isVip", cg.Key.VIP.ToString().ToLower()),
                        new XElement("orderCount", cg.Count()),
                        new XElement("totalSpent", cg.Sum(x => x.TotalAmount)),
                        new XElement("orders",
                            cg.Select(ord => new XElement("orderRef",
                                new XAttribute("id", ord.ID),
                                new XAttribute("total", ord.TotalAmount)
                            ))
                        )
                    ))
            )
        );
        return new XDocument(root);
    }
    

    public async Task SaveReportAsync(XDocument report, string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        await Task.Run(() => report.Save(path));
    }

    public async Task<IEnumerable<int>> FindHighValueOrderIdsAsync(string reportPath, decimal threshold)
    {
        if (!File.Exists(reportPath)) return Enumerable.Empty<int>();

        XDocument doc = await Task.Run(() => XDocument.Load(reportPath));

        var highValueIds = doc.Descendants("orderRef")
            .Where(x => (decimal)x.Attribute("total")! > threshold)
            .Select(x => (int)x.Attribute("id")!);

        return highValueIds;
    }
}