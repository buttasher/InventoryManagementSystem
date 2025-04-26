using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Notification
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? IsRead { get; set; }

    public int? UserId { get; set; }

    public string? Role { get; set; }
}
