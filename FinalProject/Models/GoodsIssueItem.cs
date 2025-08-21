using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class GoodsIssueItem
{
    public int IssueItemId { get; set; }

    public int IssueId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual GoodsIssue Issue { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
