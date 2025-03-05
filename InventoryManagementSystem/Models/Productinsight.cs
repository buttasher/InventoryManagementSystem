using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Productinsight
{
    public string ItemName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public int? TotalSold { get; set; }

    public decimal? Revenue { get; set; }
}
