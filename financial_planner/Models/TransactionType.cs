using System;
using System.Collections.Generic;

namespace financial_planner.Models;

public partial class TransactionType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string Color { get; set; } = null!;

    public virtual ICollection<TransactionCategory> TransactionCategories { get; set; } = new List<TransactionCategory>();
}
