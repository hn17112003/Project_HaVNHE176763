using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class GoodsReceipt
{
    public int ReceiptId { get; set; }

    public int SupplierId { get; set; }

    public DateTime? ReceiptDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; } = new List<GoodsReceiptItem>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
