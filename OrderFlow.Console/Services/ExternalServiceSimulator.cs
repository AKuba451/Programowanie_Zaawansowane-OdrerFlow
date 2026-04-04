namespace ConsoleApp1.Services;

using ConsoleApp1.Models;
using System.Diagnostics;

public class ExternalServiceSimulator
{
    private readonly Random _random = new();

    public async Task<bool> CheckInventoryAsync(Product product)
    {
        int delay = _random.Next(500, 1500);
        await Task.Delay(delay);
        return true;
    }

    public async Task<bool> ValidatePaymentAsync(Order order)
    {
        int delay = _random.Next(1000, 2000);
        await Task.Delay(delay);
        return true;
    }

    public async Task<decimal> CalculateShippingAsync(Order order)
    {
        int delay = _random.Next(300, 800);
        await Task.Delay(delay);
        return 15.50m;
    }
}