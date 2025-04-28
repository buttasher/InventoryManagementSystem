using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ItemName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public string? Brand { get; set; }

    public string? CostPrice { get; set; }

    public string SellingPrice { get; set; } = null!;

    public string? Unit { get; set; }

    public int? Quantity { get; set; }

    public DateOnly? ManufactureDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Productcategory? Category { get; set; }

    public virtual ICollection<Expireditem> Expireditems { get; set; } = new List<Expireditem>();

    public virtual ICollection<Lowstock> Lowstocks { get; set; } = new List<Lowstock>();

    public virtual ICollection<Returnitem> Returnitems { get; set; } = new List<Returnitem>();

    public virtual ICollection<Transactiondetail> Transactiondetails { get; set; } = new List<Transactiondetail>();

   
}
