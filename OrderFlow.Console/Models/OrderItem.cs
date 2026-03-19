namespace ConsoleApp1.Models;

public class OrderItem
{
    public int ID { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    
    public decimal TotalPrice => Product.Price * Quantity;
}