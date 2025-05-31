using BusinessObjects;
using System;
using System.Collections.Generic;


namespace BusinessObjects;
public partial class ShoppingCart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User User { get; set; } = null!;
}
