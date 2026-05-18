using System;
using System.Collections.Generic;

namespace financial_planner.Models;

public partial class TransactionCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TypeId { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual TransactionType Type { get; set; } = null!;
}
