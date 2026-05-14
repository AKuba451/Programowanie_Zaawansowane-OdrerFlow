using Xunit;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OrderFlow.Tests;

public class OrderValidatorTests
{
    private readonly OrderValidator _validator = new();

    [Fact]
    public void ValidateAll_OrderHasNoItems_ReturnsError()
    {
        var order = new Order 
        { 
            Items = new List<OrderItem>(),
            OrderDate = DateTime.Now.AddDays(-1)
        };

        var errors = _validator.ValidateAll(order);

        Assert.Contains("Order has no items!", errors);
    }

    [Fact]
    public void ValidateAll_OrderExceedsAmountLimit_ReturnsError()
    {
        var expensiveProduct = new Product { Price = 6000m };
        var order = new Order
        {
            Items = new List<OrderItem> 
            { 
                new OrderItem { Product = expensiveProduct, Quantity = 1, UnitPrice = 6000m } 
            },
            OrderDate = DateTime.Now.AddDays(-1)
        };

        var errors = _validator.ValidateAll(order);

        Assert.Contains("Order exceeds Maximum allowed amount!", errors);
    }

    [Fact]
    public void ValidateAll_ItemQuantityIsZero_ReturnsError()
    {
        var product = new Product { Price = 100m };
        var order = new Order
        {
            Items = new List<OrderItem> 
            { 
                new OrderItem { Product = product, Quantity = 0, UnitPrice = 100m } 
            },
            OrderDate = DateTime.Now.AddDays(-1)
        };

        var errors = _validator.ValidateAll(order);

        Assert.Contains("One or more items have invalid Quantity!", errors);
    }

    [Fact]
    public void ValidateAll_OrderDateIsInFuture_ReturnsFuncError()
    {
        var product = new Product { Price = 100m };
        var order = new Order
        {
            Items = new List<OrderItem> 
            { 
                new OrderItem { Product = product, Quantity = 1, UnitPrice = 100m } 
            },
            OrderDate = DateTime.Now.AddDays(2)
        };

        var errors = _validator.ValidateAll(order);

        Assert.Contains("Func rule failed.", errors);
    }

    [Fact]
    public void ValidateAll_MultipleViolations_ReturnsMultipleErrors()
    {
        var expensiveProduct = new Product { Price = 6000m };
        var cheapProduct = new Product { Price = 10m };
        
        var order = new Order
        {
            Status = OrderStatus.Cancelled,
            Items = new List<OrderItem> 
            { 
                new OrderItem { Product = expensiveProduct, Quantity = 1, UnitPrice = 6000m },
                new OrderItem { Product = cheapProduct, Quantity = 0, UnitPrice = 10m }
            },
            OrderDate = DateTime.Now.AddDays(5)
        };

        var errors = _validator.ValidateAll(order);

        Assert.True(errors.Count >= 3);
        Assert.Contains("Order exceeds Maximum allowed amount!", errors);
        Assert.Contains("One or more items have invalid Quantity!", errors);
    }

    [Theory]
    [InlineData(OrderStatus.New, true)]
    [InlineData(OrderStatus.Completed, true)]
    [InlineData(OrderStatus.Cancelled, false)]
    public void ValidateAll_OrderStatus_ReturnsExpectedValidation(OrderStatus status, bool expectedIsValid)
    {
        var product = new Product { Price = 100m };
        var order = new Order
        {
            Status = status,
            Items = new List<OrderItem> 
            { 
                new OrderItem { Product = product, Quantity = 1, UnitPrice = 100m } 
            },
            OrderDate = DateTime.Now.AddDays(-1)
        };

        var errors = _validator.ValidateAll(order);
        var isValid = !errors.Any();

        Assert.Equal(expectedIsValid, isValid);
    }
}