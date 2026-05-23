using ConsoleApp1.Models;
using System;

namespace ConsoleApp1.Services;

public class DiscountCalculator
{
    private const decimal VipDiscountRate = 0.10m;
    private const decimal HighValueDiscountRate = 0.05m;
    private const decimal ExtraVipHighValueDiscountRate = 0.05m;
    private const decimal MaxDiscountRate = 0.25m;
    private const decimal HighValueThreshold = 1000m;
    private const decimal ExtraVipHighValueThreshold = 5000m;

    public decimal CalculateDiscount(Order order)
    {
        decimal rate = 0m;

        if (order.Customer != null && order.Customer.VIP)
        {
            rate += VipDiscountRate;
        }

        if (order.TotalAmount > HighValueThreshold)
        {
            rate += HighValueDiscountRate;
        }

        if (order.Customer != null && order.Customer.VIP && order.TotalAmount > ExtraVipHighValueThreshold)
        {
            rate += ExtraVipHighValueDiscountRate;
        }

        if (rate > MaxDiscountRate)
        {
            rate = MaxDiscountRate;
        }

        return Math.Round(order.TotalAmount * rate, 2);
    }
}