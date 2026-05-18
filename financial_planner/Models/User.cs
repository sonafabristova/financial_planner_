using System;
using System.Collections.Generic;

namespace financial_planner.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public DateTime RegistrationDate { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
