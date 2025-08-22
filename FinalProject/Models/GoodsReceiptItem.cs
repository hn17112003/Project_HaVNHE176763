using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public partial class GoodsReceiptItem
    {
        [Key]
        [Column("receipt_item_id")]
        public int ReceiptItemId { get; set; }

        [Column("receipt_id")]
        public int ReceiptId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price", TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        // Navigation properties
        [ForeignKey("ReceiptId")]
        [InverseProperty("GoodsReceiptItems")]
        public virtual GoodsReceipt GoodsReceipt { get; set; } = null!;

        [ForeignKey("ProductId")]
        [InverseProperty("GoodsReceiptItems")]
        public virtual Product Product { get; set; } = null!;
    }
}
