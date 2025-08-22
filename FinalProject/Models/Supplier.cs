using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            GoodsReceipts = new HashSet<GoodsReceipt>();
        }

        [Key]
        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Required]
        [Column("supplier_name")]
        [StringLength(100)]
        public string SupplierName { get; set; } = null!;

        [Column("contact_person")]
        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [Column("phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("email")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Column("address")]
        [StringLength(255)]
        public string? Address { get; set; }

        // Navigation properties
        [InverseProperty("Supplier")]
        public virtual ICollection<GoodsReceipt> GoodsReceipts { get; set; }
    }
}
