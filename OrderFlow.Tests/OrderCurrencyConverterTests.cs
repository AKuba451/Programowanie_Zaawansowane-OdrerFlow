using Xunit;
using Moq;
using System.Threading.Tasks;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
using System.Collections.Generic;

namespace OrderFlow.Tests;

public class OrderCurrencyConverterTests
{
    [Fact]
    public async Task ConvertOrderTotalAsync_ReturnsConvertedValue()
    {
        var mockCurrencyService = new Mock<ICurrencyService>();
        mockCurrencyService.Setup(x => x.ConvertAsync(200m, "PLN", "EUR")).ReturnsAsync(50m);
        var converter = new OrderCurrencyConverter(mockCurrencyService.Object);
        var order = new Order
        {
            Items = new List<OrderItem> { new OrderItem { Quantity = 2, UnitPrice = 100m } }
        };

        var result = await converter.ConvertOrderTotalAsync(order, "EUR");

        Assert.Equal(50m, result);
    }

    [Fact]
    public async Task ConvertOrderTotalAsync_PropagatesExceptionFromService()
    {
        var mockCurrencyService = new Mock<ICurrencyService>();
        mockCurrencyService.Setup(x => x.ConvertAsync(It.IsAny<decimal>(), "PLN", "EUR"))
            .ThrowsAsync(new CurrencyServiceException("Test Error"));
        var converter = new OrderCurrencyConverter(mockCurrencyService.Object);
        var order = new Order
        {
            Items = new List<OrderItem> { new OrderItem { Quantity = 1, UnitPrice = 100m } }
        };

        await Assert.ThrowsAsync<CurrencyServiceException>(() => converter.ConvertOrderTotalAsync(order, "EUR"));
    }
}