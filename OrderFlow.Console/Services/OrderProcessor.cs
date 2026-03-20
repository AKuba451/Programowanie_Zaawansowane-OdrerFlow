using ConsoleApp1.Models;

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