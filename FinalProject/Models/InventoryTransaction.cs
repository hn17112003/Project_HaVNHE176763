using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public partial class InventoryTransaction
    {
        [Key]
        [Column("transaction_id")]
        public int TransactionId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("transaction_type")]
        [StringLength(50)]
        public string TransactionType { get; set; } = null!; // "Receipt", "Issue", "Adjustment"

        [Column("quantity_change")]
        public int QuantityChange { get; set; }

        [Column("transaction_date", TypeName = "datetime")]
        public DateTime? TransactionDate { get; set; }

        [Column("reference_id")]
        public int? ReferenceId { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("ProductId")]
        [InverseProperty("InventoryTransactions")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("UserId")]
        [InverseProperty("InventoryTransactions")]
        public virtual User User { get; set; } = null!;
    }
}
