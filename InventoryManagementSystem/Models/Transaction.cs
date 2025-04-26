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

    public decimal? Tax { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Paymentmethod PaymentMethod { get; set; } = null!;

    public virtual ICollection<Returnitem> Returnitems { get; set; } = new List<Returnitem>();

    public virtual ICollection<Transactiondetail> Transactiondetails { get; set; } = new List<Transactiondetail>();

    public virtual User User { get; set; } = null!;
}
