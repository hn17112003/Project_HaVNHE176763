using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class InventoryTransaction
{
    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public string TransactionType { get; set; } = null!;

    public int QuantityChange { get; set; }

    public DateTime? TransactionDate { get; set; }

    public int? ReferenceId { get; set; }

    public string? Notes { get; set; }

    public int UserId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
