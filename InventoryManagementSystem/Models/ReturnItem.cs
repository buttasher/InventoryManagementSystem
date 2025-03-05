using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class ReturnItem
{
    public int ReturnItemsId { get; set; }

    public int? TransactionId { get; set; }

    public int? ProductId { get; set; }

    public int ReturnQuantity { get; set; }

    public string? ReturnReason { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string? Status { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Transaction? Transaction { get; set; }
}
