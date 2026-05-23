using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services;

public interface ICurrencyService
{
    Task<decimal?> GetRateAsync(string currencyCode);
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
}

public class CurrencyServiceException : Exception
{
    public CurrencyServiceException(string message) : base(message) { }
}

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;
    private readonly ConcurrentDictionary<string, decimal> _cache = new(StringComparer.OrdinalIgnoreCase);

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal?> GetRateAsync(string currencyCode)
    {
        if (string.Equals(currencyCode, "PLN", StringComparison.OrdinalIgnoreCase))
        {
            return 1.0m;
        }

        if (_cache.TryGetValue(currencyCode, out var cachedRate))
        {
            return cachedRate;
        }
        
        var url = $"https://api.nbp.pl/api/exchangerates/rates/A/{currencyCode}/?format=json";
        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new CurrencyServiceException($"NBP API error: {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive =  true };
        var nbpResponse = JsonSerializer.Deserialize<NbpResponse>(json, options);

        if (nbpResponse?.Rates == null || nbpResponse.Rates.Count == 0)
        {
            throw new CurrencyServiceException("Invalid JSON response from NBP");
        }

        var rate = nbpResponse.Rates[0].Mid;
        _cache[currencyCode] = rate;

        return rate;
    }
    
    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        var fromRate = await GetRateAsync(fromCurrency);
        if (fromRate == null) throw new CurrencyServiceException($"Currency {fromCurrency} not found");

        var toRate = await GetRateAsync(toCurrency);
        if (toRate == null) throw new CurrencyServiceException($"Currency {toCurrency} not found");

        var amountInPln = amount * fromRate.Value;
        return amountInPln / toRate.Value;
    }
}

public class OrderCurrencyConverter
{
    private readonly ICurrencyService _currencyService;

    public OrderCurrencyConverter(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    public async Task<decimal> ConvertOrderTotalAsync(Order order, string targetCurrency)
    {
        return await _currencyService.ConvertAsync(order.TotalAmount, "PLN", targetCurrency);
    }
}