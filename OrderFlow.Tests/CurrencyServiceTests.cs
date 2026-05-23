using Xunit;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleApp1.Services;

namespace OrderFlow.Tests;

public class CurrencyServiceTests
{
    [Fact]
    public async Task GetRateAsync_ValidCurrency_ReturnsRate()
    {
        var handler = new TestHttpMessageHandler
        {
            ResponseContent = "{\"rates\": [{\"mid\": 4.0}]}"
        };
        var client = new HttpClient(handler);
        var service = new CurrencyService(client);

        var result = await service.GetRateAsync("USD");

        Assert.Equal(4.0m, result);
    }

    [Fact]
    public async Task GetRateAsync_PLN_ReturnsOneWithoutApiCall()
    {
        var handler = new TestHttpMessageHandler();
        var client = new HttpClient(handler);
        var service = new CurrencyService(client);

        var result = await service.GetRateAsync("PLN");

        Assert.Equal(1.0m, result);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task GetRateAsync_NotFound_ReturnsNull()
    {
        var handler = new TestHttpMessageHandler { ResponseStatusCode = HttpStatusCode.NotFound };
        var client = new HttpClient(handler);
        var service = new CurrencyService(client);

        var result = await service.GetRateAsync("XXX");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetRateAsync_ServerError_ThrowsCurrencyServiceException()
    {
        var handler = new TestHttpMessageHandler { ResponseStatusCode = HttpStatusCode.InternalServerError };
        var client = new HttpClient(handler);
        var service = new CurrencyService(client);

        await Assert.ThrowsAsync<CurrencyServiceException>(() => service.GetRateAsync("USD"));
    }

    [Fact]
    public async Task ConvertAsync_TwoDifferentCurrencies_ReturnsConvertedAmount()
    {
        var handler = new TestHttpMessageHandler
        {
            ResponseContent = "{\"rates\": [{\"mid\": 4.0}]}"
        };
        var client = new HttpClient(handler);
        var service = new CurrencyService(client);
        
        var result = await service.ConvertAsync(100m, "PLN", "USD");

        Assert.Equal(25m, result);
    }

    [Fact]
    public async Task GetRateAsync_ChecksCorrectUrl()
    {
        var handler = new TestHttpMessageHandler
        {
            ResponseContent = "{\"rates\": [{\"mid\": 4.0}]}"
        };
        var client = new HttpClient(handler);
        var service = new CurrencyService(client);

        await service.GetRateAsync("EUR");

        Assert.NotNull(handler.LastRequest);
        Assert.Equal("https://api.nbp.pl/api/exchangerates/rates/A/EUR/?format=json", handler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task GetRateAsync_Bonus_UsesCacheForRepeatedCalls()
    {
        var handler = new TestHttpMessageHandler
        {
            ResponseContent = "{\"rates\": [{\"mid\": 4.0}]}"
        };
        var client = new HttpClient(handler);
        var service = new CurrencyService(client);

        await service.GetRateAsync("USD");
        await service.GetRateAsync("USD");

        Assert.Equal(1, handler.RequestCount);
    }
}