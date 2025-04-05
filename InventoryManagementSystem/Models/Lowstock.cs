using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Lowstock
{
    public int LowStockId { get; set; }

    public int ProductId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Category { get; set; }

    public string? Brand { get; set; }

    public int Quantity { get; set; }

    public short Alert { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
