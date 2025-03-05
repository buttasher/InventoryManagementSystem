using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string ContactNumer { get; set; } = null!;

    public string Age { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
