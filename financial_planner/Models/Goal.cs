using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace financial_planner.Models
{
    public partial class Goal
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
        public int AllocationPercentage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public virtual Priority Priority { get; set; } = null!;
        public virtual GoalStatus Status { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        // Вычисляемые свойства (не сохраняются в БД)
        [NotMapped]
        public double Progress => TargetAmount > 0 ? (double)(CurrentAmount / TargetAmount * 100) : 0;

        [NotMapped]
        public decimal Remaining => TargetAmount - CurrentAmount;

        [NotMapped]
        public string ProgressText => $"{Progress:F1}%";

        [NotMapped]
        public string DisplayName => $"{Name} ({CurrentAmount:N0}/{TargetAmount:N0})";
    }
}