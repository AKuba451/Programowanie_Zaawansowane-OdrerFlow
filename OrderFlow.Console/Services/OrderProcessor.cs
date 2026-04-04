using ConsoleApp1.Models;
using System.Diagnostics;

namespace ConsoleApp1.Services;

public class OrderProcessor
{
    public List<Order> FilterOrders(List<Order> orders, Predicate<Order> condition)
    {
        return orders.FindAll(condition);
    }

    public void ProcessOrders(List<Order> orders, Action<Order> action)
    {
        foreach (var order in orders)
        {
            action(order);
        }
    }

    public List<T> ProjectOrders<T>(List<Order> orders, Func<Order, T> selector)
    {
        var results = new List<T>();
        foreach (var order in orders)
        {
            results.Add(selector(order));
        }
        return results;
    }

    public decimal AggregateOrders(List<Order> orders, Func<IEnumerable<Order>, decimal> aggregator)
    {
        return aggregator(orders);
    }
}

public class AsyncOrderProcessor
{
    private readonly ExternalServiceSimulator _service = new();
    private readonly SemaphoreSlim _semaphore = new(3);

    public async Task ProcessOrderAsync(Order order)
    {
        var sw = Stopwatch.StartNew();

        var inventoryTask = _service.CheckInventoryAsync(order.Items.First().Product);
        var paymentTask = _service.ValidatePaymentAsync(order);
        var shippingTask = _service.CalculateShippingAsync(order);
        
        await Task.WhenAll(inventoryTask, paymentTask, shippingTask);
        
        sw.Stop();
        Console.WriteLine($"[ASYNC] Zamowienie {order.ID} przetworzone w {sw.ElapsedMilliseconds} ms");
    }

    public async Task ProcessMultipleOrdersAsync(List<Order> orders)
    {
        int processedCount = 0;
        var tasks = orders.Select(async order =>
        {
            await _semaphore.WaitAsync();
            try
            {
                await ProcessOrderAsync(order);
                Interlocked.Increment(ref processedCount);
                Console.WriteLine($"Postep: Przetworzono {processedCount}/{orders.Count} zamowien");
            }
            finally
            {
                _semaphore.Release();
            }
        });
        
        await Task.WhenAll(tasks);
    }
}