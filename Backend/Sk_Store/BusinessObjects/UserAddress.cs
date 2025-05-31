using System;
using System.Collections.Generic;


namespace BusinessObjects;
public partial class UserAddress
{
    public int AddressId { get; set; }

    public int UserId { get; set; }

    public string AddressLine { get; set; } = null!;

    public string? RecipientName { get; set; }

    public string? RecipientPhoneNumber { get; set; }

    public bool IsDefault { get; set; }

    public virtual User User { get; set; } = null!;
}
