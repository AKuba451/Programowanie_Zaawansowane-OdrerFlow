namespace ConsoleApp1.Models;

public class Order
{
    public int ID { get; set; }
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
    public OrderStatus Status { get; set; }
    
    public decimal TotalAmmount => Items.Sum(i => i.TotalPrice);
}