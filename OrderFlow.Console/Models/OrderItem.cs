namespace ConsoleApp1.Models;

public class OrderItem
{
    public int ID { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
    public decimal TotalPrice => UnitPrice * Quantity;
}