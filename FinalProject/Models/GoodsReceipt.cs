using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public partial class GoodsReceipt
    {
        public GoodsReceipt()
        {
            GoodsReceiptItems = new HashSet<GoodsReceiptItem>();
        }

        [Key]
        [Column("receipt_id")]
        public int ReceiptId { get; set; }

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("receipt_date", TypeName = "datetime")]
        public DateTime? ReceiptDate { get; set; }

        [Column("total_amount", TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column("status")]
        [StringLength(50)]
        public string? Status { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        // Navigation properties
        [ForeignKey("SupplierId")]
        [InverseProperty("GoodsReceipts")]
        public virtual Supplier Supplier { get; set; } = null!;

        [ForeignKey("UserId")]
        [InverseProperty("GoodsReceipts")]
        public virtual User User { get; set; } = null!;

        [InverseProperty("GoodsReceipt")]
        public virtual ICollection<GoodsReceiptItem> GoodsReceiptItems { get; set; }
    }
}
