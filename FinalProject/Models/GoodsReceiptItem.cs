using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class GoodsReceiptItem
{
    public int ReceiptItemId { get; set; }

    public int ReceiptId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual GoodsReceipt Receipt { get; set; } = null!;
}
