using System;
using System.Collections.Generic;

namespace financial_planner.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public string? Note { get; set; }

    public virtual TransactionCategory Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
