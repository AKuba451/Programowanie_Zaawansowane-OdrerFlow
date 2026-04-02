using ConsoleApp1.Data;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System.Linq;
// POCZATEK ZADANIA 2
var validator = new OrderValidator();

var validOrder = SampleData.Orders[0];
var validResult = validator.ValidateAll(validOrder);

Console.WriteLine("VALID ORDER: ");
if (!validResult.Any())
{
    Console.WriteLine("ORDER IS VALID!");
}
else
{
    validResult.ForEach(Console.WriteLine);
}


var invalidOrder = new Order
{
    ID = 9999,
    Customer = SampleData.Customers[0],
    Status = OrderStatus.Cancelled,
    Items = new List<OrderItem>
    {
        new OrderItem
        {
            ID = 100,
            Product = SampleData.Products[0],
            Quantity = 0
        }
    }
};

var invalidResult =  validator.ValidateAll(invalidOrder);

Console.WriteLine("INVALID ORDER: ");
if (!invalidResult.Any())
{
    Console.WriteLine("ORDER IS VALID!");
}
else
{
    invalidResult.ForEach(Console.WriteLine);
}

// KONIEC ZADANIA 2

// POCZATEK ZADANIA 3
var processor = new OrderProcessor();
var allOrders = SampleData.Orders;

var expensiveOrders = processor.FilterOrders(allOrders, o => o.TotalAmount > 1000);
var vipOrders = processor.FilterOrders(allOrders, o=> o.Customer.VIP);
var newOrders = processor.FilterOrders(allOrders, o => o.Status == OrderStatus.New);

Action<Order> printSummary = o => Console.WriteLine($"ID: {o.ID}, Customer: {o.Customer.Name}, Total:  {o.TotalAmount:C}");
Action<Order> markAsProcessing = o => o.Status = OrderStatus.Processing;

Console.WriteLine("\n--- PRINT : ZADANIE 3 ---");
processor.ProcessOrders(newOrders, printSummary);

var shortSummaries = processor.ProjectOrders(allOrders, o => new
{
    OrderID = o.ID,
    CustomerName = o.Customer.Name,
    ItemCount = o.Items.Count,
});

decimal total = processor.AggregateOrders(allOrders, items => items.Sum(o => o.TotalAmount));
decimal average = processor.AggregateOrders(allOrders, items => items.Average(o => o.TotalAmount));
decimal max = processor.AggregateOrders(allOrders, items => items.Max(o => o.TotalAmount));

Console.WriteLine($"\nSTATS: TOTAL: {total:C}, AVERAGE: {average:C}, MAX: {max:C}");

Console.WriteLine("\n--- ODPALENIE : ZADANIE 3 ---");
allOrders
    .Where(o => o.Status != OrderStatus.Cancelled)
    .OrderByDescending(o => o.TotalAmount)
    .Take(3)
    .ToList()
    .ForEach(o => Console.WriteLine($"TOP - ID: {o.ID}, TOTAL: {o.TotalAmount:C}"));

// KONIEC ZADANIA 3
    
// POCZATEK ZADANIA 4

var products = SampleData.Products;
var customers = SampleData.Customers;
var orders = SampleData.Orders;
    
Console.WriteLine("\n === LINQ : ZADANIE 4 === \n");

//1. Join Zamowien z Klientami - Grupowanie zamowien po statusie VIP klienta
//Wybralem Query Syntax, poniewaz joiny sa w tej skladni bardziej czytelne ( przypominaja troche SQL )

var ordersByVipStatus = from o in orders
    join c in customers on o.Customer.ID equals c.ID
    group o by c.VIP
    into g
    select new { IsVip = g.Key, Orders = g.ToList() };
    
    Console.WriteLine("1. Grupowanie po VIP: ");
    foreach (var group in ordersByVipStatus)
    {
        Console.WriteLine($"VIP : {group.IsVip}, Liczba zamowien: {group.Orders.Count}");
    }

//2. Splaszczanie z SelectMany - Wyciagniecie wszystkich unikalnych nazw produktow z zamowien
//Wybralem Method Syntax, poniewaz SelectMany jest znacznie krotsze i prostsze w tej formie.

    var allOrderedProducts = orders
        .SelectMany(o => o.Items)
        .Select(i => i.Product.Name)
        .Distinct();

Console.WriteLine($"\n2. Wszystkie Zamowione produkty: {string.Join(", ", allOrderedProducts)}");

//3. GroupBy z agregacja - Srednia Wartosc zamowienia per kategoria produktu
//Wybralem Method Syntax poniewaz jest to idealna opcja do szybkich agregacji takich jak Average czy Sum.

var avgValuePerCategory = orders
    .SelectMany(o => o.Items)
    .GroupBy(o => o.Product.Category)
    .Select(g => new { Category = g.Key, AvgPrice = g.Average(x => x.TotalPrice) });

Console.WriteLine("\n3. Srednia wartosc per kategoria: ");
foreach (var item in avgValuePerCategory)
{
    Console.WriteLine($"Kategoria: {item.Category}, Srednia: {item.AvgPrice:C}");
}

//4. GroupJoin (Left Join Pattern) - Lista klientow i ich zamowienia (nawet jezeli nic nie kupili)
//Wybralem Query Syntax poniewaz w polaczeniu z into jest idealna opcja do wykonania GroupJoin

var CustomerLeads = from c in customers
                    join o in orders on c.ID equals o.Customer.ID into customerOrders
                    select new { CustomerName = c.Name, Ordercount = customerOrders.Count() };

Console.WriteLine("\n4. Klienci i liczba ich zamowien (GroupJoin): ");
CustomerLeads.ToList().ForEach(cl => Console.WriteLine($"Klient: {cl.CustomerName}, Zamowienia:  {cl.Ordercount}"));

//5. Mixed Syntax - Raport: Klient i jego najdrozsze zamowienie
//Polaczenie Query Syntax (do czytelnego polaczenia danych) z Method Syntax (do wyciagniecia Max).

var customerReport = (from c in customers
    join o in orders on c.ID equals o.Customer.ID into oGroup
    select new
    {
        Name = c.Name,
        MaxOrder = oGroup.DefaultIfEmpty().Max(x => x?.TotalAmount ?? 0)
    }).OrderByDescending(r => r.MaxOrder);

Console.WriteLine("\n5. Najdrozsze zamowienie klienta (MixedSyntax):");
foreach (var r in customerReport)
{
    Console.WriteLine($" Klient: {r.Name}, Max: {r.MaxOrder:C}");
}

//6. Proste filtrowanie i sortowanie - Top 2 najdrozsze produkty z kategorii Electronics
//Wybralem Method Syntax poniewaz jest to najlepsza opcja do stosowania w prostych operacjach filtrowania

var topElectronics = products
    .Where(p => p.Category == "Electronics")
    .OrderByDescending(p => p.Price)
    .Take(2);

Console.WriteLine("\n6. TOP 2 Elektronika: ");
foreach (var p in topElectronics)
{
    Console.WriteLine($"Produkt: {p.Name}, Cena: {p.Price:C}");
}
    
//KONIEC ZADANIA 4

//POCZATEK ZADANIA 2.1

var pipeline = new OrderPipeline();

pipeline.StatusChanged += (sender, e) => 
    Console.WriteLine($"[LOGGER] Zamowienie {e.Order.ID}: {e.OldStatus} -> {e.NewStatus}");
    pipeline.StatusChanged += (sender, e) =>
    {
        if (e.NewStatus == OrderStatus.Completed)
            Console.WriteLine($"[EMAIL] Do: {e.Order.Customer.Name} Twoje zamowienie {e.Order.ID} zostalo ukonczone !");
    };
    
    pipeline.ValidationCompleted += (sender, e) =>
    {
        if (e.isValid)
        {
            Console.WriteLine($"[VALIDATOR] Zamowienie {e.Order.ID} przeszlo pomyslnie walidacje");
        }
        else
        {
            Console.WriteLine($"[VALIDATOR] Blad w zamowieniu {e.Order.ID}: {string.Join(" ,", e.Errors)}");
        }
    };
    
    Console.WriteLine("=== LABORATORIUM 2 --- ZADANIE 1 ===\n");
    Console.WriteLine("--- Przetwarzanie Zamowienia 1 ( POPRAWNE ) ---");
    pipeline.ProcessOrder(SampleData.Orders[0]);
    Console.WriteLine("--- Przetwarzanie Zamowienia 2 ( NIEPOPRAWNE - ANULOWANE ) ---");
    pipeline.ProcessOrder(SampleData.Orders[5]);