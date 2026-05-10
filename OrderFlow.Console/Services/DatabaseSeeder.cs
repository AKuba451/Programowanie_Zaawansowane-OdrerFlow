using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Data;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(OrderFlowContext db)
    {
        if (!await db.Orders.AnyAsync())
        {
            foreach (var order in SampleData.Orders)
            {
                foreach (var item in order.Items)
                {
                    item.UnitPrice = item.Product.Price;
                }
            }
            
            await db.Customers.AddRangeAsync(SampleData.Customers);
            await db.Products.AddRangeAsync(SampleData.Products);
            await db.Orders.AddRangeAsync(SampleData.Orders);
            
            await db.SaveChangesAsync();
            
            Console.WriteLine("[SEEDER] Baza danych została pomyślnie wypełniona !");
        }
        else
        {
            Console.WriteLine("[SEEDER] Baza danych zawiera już dane !");
        }
    }
}