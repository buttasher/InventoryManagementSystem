using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public int PaymentMethodId { get; set; }

    public int CustomerId { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();

    public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();

    public virtual User User { get; set; } = null!;
}
