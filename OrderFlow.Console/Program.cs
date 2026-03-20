using System.Collections.Specialized;
using ConsoleApp1.Data;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
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