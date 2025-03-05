using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Password { get; set; }

    public int? PhoneNumber { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
