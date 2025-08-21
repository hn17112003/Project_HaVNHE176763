using System;
using System.Collections.Generic;

namespace FinalProject.Models;

public partial class GoodsIssue
{
    public int IssueId { get; set; }

    public DateTime? IssueDate { get; set; }

    public string IssueReason { get; set; } = null!;

    public string? Status { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<GoodsIssueItem> GoodsIssueItems { get; set; } = new List<GoodsIssueItem>();

    public virtual User User { get; set; } = null!;
}
