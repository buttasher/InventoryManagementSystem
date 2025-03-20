using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Paymentmethod
{
    public int PaymentMethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
