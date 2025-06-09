using System;
using System.Collections.Generic;


namespace BusinessObjects;
public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    public bool IsActive { get; set; }

    public DateTime RegistrationDate { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool IsVerified { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role Role { get; set; } = null!;

    public virtual ShoppingCart? ShoppingCart { get; set; }

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}
