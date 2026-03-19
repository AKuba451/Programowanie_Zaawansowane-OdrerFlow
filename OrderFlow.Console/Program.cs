using System.Collections.Specialized;
using ConsoleApp1.Data;
using ConsoleApp1.Models;
using ConsoleApp1.Services;

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