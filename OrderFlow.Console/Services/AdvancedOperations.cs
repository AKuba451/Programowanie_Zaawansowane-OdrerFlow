using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Data;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services;

public class AdvancedOperations
{
    public async Task RunAdvancedQueriesAsync(OrderFlowContext db)
    {
        Console.WriteLine("\n--- ZAPYTANIA LINQ (IQueryable) ---");

        var vipBigOrders = await db.Orders
            .Where(o => o.Customer.VIP && o.Items.Sum(i => i.Quantity * i.UnitPrice) > 1000)
            .Select(o => new { o.ID, o.Customer.Name, Total = o.Items.Sum(i => i.Quantity * i.UnitPrice) })
            .ToListAsync();

        Console.WriteLine("\n1. Zamówienia VIP powyżej 1000:");
        vipBigOrders.ForEach(o => Console.WriteLine($" - ID: {o.ID}, Klient: {o.Name}, Kwota: {o.Total:C}"));

        var rawCustomersData = await db.Orders
            .GroupBy(o => o.Customer.Name)
            .Select(g => new 
            { 
                CustomerName = g.Key, 
                TotalSpent = g.Sum(o => o.Items.Sum(i => i.Quantity * i.UnitPrice)) 
            })
            .ToListAsync();

        var topCustomers = rawCustomersData
            .OrderByDescending(x => x.TotalSpent)
            .ToList();

        Console.WriteLine("\n2. Ranking klientów wg wydatków:");
        topCustomers.ForEach(c => Console.WriteLine($" - {c.CustomerName}: {c.TotalSpent:C}"));

        var rawOrdersPerCity = await db.Orders
            .Where(o => o.Customer.City != null)
            .Select(o => new 
            {
                City = o.Customer.City,
                OrderValue = o.Items.Sum(i => i.Quantity * i.UnitPrice)
            })
            .ToListAsync();

        var avgPerCity = rawOrdersPerCity
            .GroupBy(x => x.City)
            .Select(g => new 
            { 
                City = g.Key, 
                AvgValue = g.Average(x => x.OrderValue) 
            })
            .ToList();

        Console.WriteLine("\n3. Średnia wartość zamówienia per miasto:");
        avgPerCity.ForEach(c => Console.WriteLine($" - {c.City}: {c.AvgValue:C}"));

        var unsoldProducts = await db.Products
            .Where(p => !db.OrderItems.Any(oi => oi.Product.ID == p.ID))
            .ToListAsync();

        Console.WriteLine("\n4. Produkty, które nigdy nie zostały zamówione:");
        if (!unsoldProducts.Any()) Console.WriteLine(" - Brak! Wszystko schodzi.");
        unsoldProducts.ForEach(p => Console.WriteLine($" - {p.Name} (Cena: {p.Price:C})"));

        RunDynamicQuery(db, OrderStatus.New, 500m);
    }

    private void RunDynamicQuery(OrderFlowContext db, OrderStatus? statusFilter, decimal? minAmount)
    {
        IQueryable<Order> query = db.Orders.Include(o => o.Customer);

        if (statusFilter.HasValue)
        {
            query = query.Where(o => o.Status == statusFilter.Value);
        }

        if (minAmount.HasValue)
        {
            query = query.Where(o => o.Items.Sum(i => i.Quantity * i.UnitPrice) >= minAmount.Value);
        }

        var results = query.ToList();
        
        Console.WriteLine($"\n5. Dynamiczne zapytanie (Status: {statusFilter}, Min: {minAmount:C}):");
        results.ForEach(o => Console.WriteLine($" - ID: {o.ID}, Klient: {o.Customer.Name}, Status: {o.Status}"));
    }

    public async Task ProcessOrderAsync(OrderFlowContext db, int orderId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var order = await db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.ID == orderId);

            if (order == null) throw new Exception("Nie znaleziono zamówienia!");

            order.Status = OrderStatus.Processing;
            await db.SaveChangesAsync();

            foreach(var item in order.Items)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    throw new InvalidOperationException($"Brak na magazynie! Produkt: '{item.Product.Name}'. Wymagane: {item.Quantity}, Dostępne: {item.Product.Stock}.");
                }
                item.Product.Stock -= item.Quantity;
            }

            order.Status = OrderStatus.Completed;
            await db.SaveChangesAsync();

            await transaction.CommitAsync();
            Console.WriteLine($"[Transakcja Sukces] Zamówienie {orderId} zrealizowane. Stany magazynowe zaktualizowane!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"[Transakcja Rollback] Błąd w zamówieniu {orderId}: {ex.Message}");
            throw;
        }
    }
}