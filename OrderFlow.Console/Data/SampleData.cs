using ConsoleApp1.Models;

namespace ConsoleApp1.Data;

public static class SampleData
{
    public static List<Product> Products = new List<Product>
    {
        new Product { ID = 1, Name = "PC", Category = "Electronics", Price = 2000 },
        new Product { ID = 2, Name = "SmartPhone", Category = "Electronics", Price = 800 },
        new Product { ID = 3, Name = "Keyboard", Category = "Accessory", Price = 120 },
        new Product { ID = 4, Name = "Mouse", Category = "Accessory", Price = 80 },
        new Product { ID = 5, Name = "Speaker", Category = "MusicRelated", Price = 100 }
    };

    public static List<Customer> Customers = new List<Customer>
    {
        new Customer {ID = 1,Name = "Jakub",VIP = true},
        new Customer {ID = 2,Name = "Bartosz",VIP = false},
        new Customer {ID = 3,Name = "Julia",VIP = false},
        new Customer {ID = 4, Name = "Igor",VIP = true},
        new Customer {ID = 5, Name = "Jacek",VIP = false}
    };

    public static List<Order> Orders = new List<Order>
    {
        new Order
        {
            ID = 1,
            Customer = Customers[0],
            Status = OrderStatus.New,
            Items = new List<OrderItem>
            {
                new OrderItem {ID = 1, Product = Products[0],Quantity = 2},
                new OrderItem {ID = 2, Product = Products[1],Quantity = 1},
            }
        },
        new Order
        {
            ID = 2,
            Customer = Customers[1],
            Status = OrderStatus.Processing,
            Items = new List<OrderItem>
            {
                new OrderItem {ID = 3, Product = Products[2],Quantity = 1},
                new OrderItem {ID = 4, Product = Products[1],Quantity = 2},
            }
        },
        new Order
        {
            ID = 3,
            Customer = Customers[3],
            Status = OrderStatus.Completed,
            Items = new List<OrderItem>
            {
                new OrderItem {ID = 5, Product = Products[0],Quantity = 1 },
                new OrderItem {ID = 6, Product = Products[1],Quantity = 1},
                new OrderItem {ID = 7, Product = Products[4],Quantity = 2},
            }
        },
        new Order
        {
            ID = 4,
            Customer = Customers[4],
            Status = OrderStatus.Validated,
            Items = new List<OrderItem>
            {
                new OrderItem { ID = 8,Product = Products[2],Quantity = 1 },
                new OrderItem{ ID = 9, Product = Products[4],Quantity = 1 },
            }
        },
        new Order
        {
            ID = 5,
            Customer = Customers[1],
            Status = OrderStatus.New,
            Items = new List<OrderItem>
            {
                new OrderItem{ ID = 10, Product = Products[0],Quantity = 2},
                new OrderItem{ ID = 11, Product = Products[3],Quantity = 1}
            }
        },
        new Order
        {
            ID = 6,
            Customer = Customers[2],
            Status = OrderStatus.Cancelled,
            Items = new List<OrderItem>
            {
                new OrderItem{ID = 12, Product = Products[1],Quantity = 2}
            }
        }
        
    };
}