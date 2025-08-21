using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class InventoryAdjustment
{
    public int AdjustmentId { get; set; }

    public int ProductId { get; set; }

    public DateTime? AdjustmentDate { get; set; }

    public int AdjustedQuantity { get; set; }

    public string Reason { get; set; } = null!;

    public int UserId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
