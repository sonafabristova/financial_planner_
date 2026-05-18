using System;
using System.Collections.Generic;

namespace financial_planner.Models;

public partial class Priority
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
}
