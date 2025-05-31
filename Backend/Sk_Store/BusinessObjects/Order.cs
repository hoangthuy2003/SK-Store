using System;
using System.Collections.Generic;



namespace BusinessObjects;
public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string RecipientPhoneNumber { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime? DeliveryDate { get; set; }

    public decimal? ShippingFee { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
