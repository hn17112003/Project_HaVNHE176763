using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class Shipping
{
    public int ShippingId { get; set; }

    public int? OrderId { get; set; }

    public string? TrackingNumber { get; set; }

    public DateTime? ShippingDate { get; set; }

    public string? Status { get; set; }

    public virtual Order? Order { get; set; }
}
