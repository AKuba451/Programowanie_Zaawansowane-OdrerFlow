using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ConsoleApp1.Models;

public class Order
{
    [XmlAttribute("OrderId")]
    [JsonPropertyName("order_id")]
    public int ID { get; set; }
    [XmlElement("Customer")]
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
    public OrderStatus Status { get; set; }
    
    [JsonIgnore]
    [XmlIgnore]
    public decimal TotalAmount => Items?.Sum(i => i.TotalPrice) ?? 0;
}