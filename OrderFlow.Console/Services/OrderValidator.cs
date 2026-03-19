using ConsoleApp1.Models;

namespace ConsoleApp1.Services;

public delegate bool ValidationRule(Order order, out string errorMessage);

public class OrderValidator
{
    private readonly List<ValidationRule> _rules = new();
    private readonly List<Func<Order, bool>> _funcRules = new();

    public OrderValidator()
    {
        _rules.Add(HasItems);
        _rules.Add(TotalAmountBelowLimit);
        _rules.Add(AllQuantitiesGreaterThanZero);
        
        _funcRules.Add(o => o.Status != OrderStatus.Cancelled);
        _funcRules.Add(o => o.Items.All(i => i.Product != null));
    }

    private bool HasItems(Order order, out string errorMessage)
    {
        if (order.Items == null || !order.Items.Any())
        {
            errorMessage = "Order has no items!";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    private bool TotalAmountBelowLimit(Order order, out string errorMessage)
    {
        if (order.TotalAmount > 5000)
        {
            errorMessage = "Order exceeds Maximum allowed amount!";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    private bool AllQuantitiesGreaterThanZero(Order order, out string errorMessage)
    {
        if (order.Items.Any(i => i.Quantity <= 0))
        {
            errorMessage = "One or more items have invalid Quantity!";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    public List<string> ValidateAll(Order order)
    {
        var errors = new List<string>();
        foreach (var rule in _rules)
        {
            if (!rule(order, out string error))
            {
                errors.Add(error);
            }
        }

        foreach (var funcRule in _funcRules)
        {
            if (!funcRule(order))
            {
                errors.Add("Func rule failed.");
            }
        }
        return errors;
    }
}