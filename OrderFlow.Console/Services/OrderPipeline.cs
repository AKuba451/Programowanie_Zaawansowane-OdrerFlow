namespace ConsoleApp1.Services;

using ConsoleApp1.Models;

public class OrderPipeline
{
    private readonly OrderValidator _validator = new();
    
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<OrderValidationEventArgs>? ValidationCompleted;

    public void ProcessOrder(Order order)
    {
        //Walidacja
        var errors = _validator.ValidateAll(order);
        bool isValid = !errors.Any();
        
        ValidationCompleted?.Invoke(this, new OrderValidationEventArgs(order, isValid, errors));

        if (!isValid) return;
        
        //Statusy

        ChangeStatus(order, OrderStatus.Validated);
        ChangeStatus(order, OrderStatus.Processing);
        ChangeStatus(order, OrderStatus.Completed);
    }

    private void ChangeStatus(Order order, OrderStatus newStatus)
    {
        var oldStatus = order.Status;
        order.Status = newStatus;
        
        StatusChanged?.Invoke(this, new OrderStatusChangedEventArgs(order, oldStatus, newStatus));
    }
}