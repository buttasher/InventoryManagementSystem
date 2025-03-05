using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Expireditem
{
    public int ProductId { get; set; }

    public string ItemName { get; set; } = null!;

    public string ItemId { get; set; } = null!;

    public int? CategoryId { get; set; }

    public string? Brand { get; set; }

    public string? Unit { get; set; }

    public int? Quantity { get; set; }

    public DateOnly? ManufactureDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateTime? CreatedAt { get; set; }
}
