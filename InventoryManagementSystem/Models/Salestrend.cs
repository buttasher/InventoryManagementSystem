using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Salestrend
{
    public DateOnly? Date { get; set; }

    public decimal? TotalSales { get; set; }
}
