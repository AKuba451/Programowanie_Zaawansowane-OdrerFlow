namespace ConsoleApp1.Models;

public class OrderStatusChangedEventArgs : EventArgs
{
    public Order Order { get; } 
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }

    public OrderStatusChangedEventArgs(Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        Order = order;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        
    }
}

public class OrderValidationEventArgs : EventArgs
{
    public Order Order { get; }
    public bool isValid { get; }
    public List<string> Errors { get; }

    public OrderValidationEventArgs(Order order, bool isValid, List<string> errors)
    {
        Order = order;
        this.isValid = isValid;
        Errors = errors;    
    }
}