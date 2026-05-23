using Xunit;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System.Collections.Generic;

namespace OrderFlow.Tests;

public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _calculator = new();

    [Fact]
    public void CalculateDiscount_StandardCustomerSmallAmount_ReturnsZero()
    {
        var order = CreateOrder(false, 500m);
        var result = _calculator.CalculateDiscount(order);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateDiscount_VipCustomerSmallAmount_Returns10Percent()
    {
        var order = CreateOrder(true, 500m);
        var result = _calculator.CalculateDiscount(order);
        Assert.Equal(50m, result);
    }

    [Fact]
    public void CalculateDiscount_StandardCustomerOver1000_Returns5Percent()
    {
        var order = CreateOrder(false, 1500m);
        var result = _calculator.CalculateDiscount(order);
        Assert.Equal(75m, result);
    }

    [Fact]
    public void CalculateDiscount_VipCustomerOver1000_Returns15Percent()
    {
        var order = CreateOrder(true, 1500m);
        var result = _calculator.CalculateDiscount(order);
        Assert.Equal(225m, result);
    }

    [Fact]
    public void CalculateDiscount_VipCustomerOver5000_Returns20Percent()
    {
        var order = CreateOrder(true, 6000m);
        var result = _calculator.CalculateDiscount(order);
        Assert.Equal(1200m, result);
    }

    [Fact]
    public void CalculateDiscount_StandardCustomerOver5000_Returns5Percent()
    {
        var order = CreateOrder(false, 6000m);
        var result = _calculator.CalculateDiscount(order);
        Assert.Equal(300m, result);
    }

    [Fact]
    public void CalculateDiscount_MaxDiscountCappedAt25Percent()
    {
        var order = CreateOrder(true, 6000m);
        var result = _calculator.CalculateDiscount(order);
        Assert.True(result <= 1500m);
    }

    private Order CreateOrder(bool isVip, decimal totalAmount)
    {
        return new Order
        {
            Customer = new Customer { VIP = isVip },
            Items = new List<OrderItem>
            {
                new OrderItem { Product = new Product { Price = totalAmount }, Quantity = 1, UnitPrice = totalAmount }
            }
        };
    }
}