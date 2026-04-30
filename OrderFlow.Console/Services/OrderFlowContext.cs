using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Models;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.Services;

public class OrderFlowContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite("Data Source = orderflow.db")
            .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().Ignore(o => o.TotalAmount);
        modelBuilder.Entity<OrderItem>().Ignore(oi => oi.TotalPrice);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>()
            .WithMany(o => o.Items)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .IsRequired();

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Name);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.Status);
    }
}