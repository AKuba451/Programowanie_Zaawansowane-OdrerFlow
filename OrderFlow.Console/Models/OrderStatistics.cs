namespace ConsoleApp1.Models;

using ConsoleApp1.Models;
using System.Collections.Concurrent;

public class OrderStatistics
{
    private readonly object _lock = new();

    public int TotalProcessed = 0;
    public decimal TotalRevenue = 0;

    public ConcurrentDictionary<OrderStatus, int> OrdersPerStatus = new();

    public List<string> ProcessingErrors = new();

    public void UpdateSafe(Order order, bool isValid, List<string> errors)
    {
        Interlocked.Increment(ref TotalProcessed);

        lock (_lock)
        {
            TotalRevenue += order.TotalAmount;
            if (!isValid)
            {
                ProcessingErrors.AddRange(errors);
            }
        }

        OrdersPerStatus.AddOrUpdate(order.Status, 1, (status, oldVal) => oldVal + 1);
    }

    public void UpdateUnsafe(Order order, bool isValid, List<string> errors)
    {
        TotalProcessed++;
        TotalRevenue += order.TotalAmount;
        if (!isValid) ProcessingErrors.AddRange(errors);

        int currentCount = OrdersPerStatus.GetOrAdd(order.Status, 0);
        OrdersPerStatus[order.Status] = currentCount + 1;
    }

    public void Reset()
    {
        TotalProcessed = 0;
        TotalRevenue = 0;
        OrdersPerStatus.Clear();
        ProcessingErrors.Clear();
    }
}