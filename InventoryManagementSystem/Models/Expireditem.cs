using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Expireditem
{
    public int ExpiredItemId { get; set; }

    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Category { get; set; }

    public DateOnly ManufactureDate { get; set; }

    public DateOnly ExpireDate { get; set; }

    public short Alert { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Item { get; set; } = null!;
}
