using System;
using System.Collections.Generic;

namespace financial_planner.Models;

public partial class Account
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal CurrentBalance { get; set; }

    public decimal MonthlyIncome { get; set; }

    public decimal MonthlyExpenses { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual User User { get; set; } = null!;
}
