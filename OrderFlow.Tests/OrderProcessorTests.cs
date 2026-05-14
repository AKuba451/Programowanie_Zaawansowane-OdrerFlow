using Xunit;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System.Collections.Generic;
using System.Linq;

namespace OrderFlow.Tests;

public class OrderProcessorTests
{
    private readonly OrderProcessor _processor = new();

    [Fact]
    public void FilterOrders_ByStatus_ReturnsOnlyMatchingOrders()
    {
        var orders = new List<Order>
        {
            new Order { ID = 1, Status = OrderStatus.New },
            new Order { ID = 2, Status = OrderStatus.Completed },
            new Order { ID = 3, Status = OrderStatus.New }
        };

        var result = _processor.FilterOrders(orders, o => o.Status == OrderStatus.New);

        Assert.Equal(2, result.Count);
        Assert.All(result, o => Assert.Equal(OrderStatus.New, o.Status));
    }

    [Fact]
    public void AggregateOrders_SumTotalAmount_ReturnsCorrectSum()
    {
        var product1 = new Product { Price = 50m };
        var product2 = new Product { Price = 100m };
        
        var orders = new List<Order>
        {
            new Order { Items = new List<OrderItem> { new OrderItem { Product = product1, Quantity = 2, UnitPrice = 50m } } },
            new Order { Items = new List<OrderItem> { new OrderItem { Product = product2, Quantity = 1, UnitPrice = 100m } } }
        };

        var result = _processor.AggregateOrders(orders, list => list.Sum(o => o.TotalAmount));

        Assert.Equal(200m, result);
    }
}