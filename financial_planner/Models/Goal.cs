using System;

namespace financial_planner.Models
{
    public class Goal
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public Priority Priority { get; set; }
        public GoalStatus Status { get; set; }
        public int AllocationPercentage { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool CanAllocate => Status?.CanAllocate ?? false;
        public bool IsCompleted => Status?.Id == GoalStatus.Completed.Id;
        public bool IsActive => Status?.Id == GoalStatus.Active.Id;

        public double Progress => TargetAmount > 0
            ? (double)(CurrentAmount / TargetAmount * 100)
            : 0;

        public decimal Remaining => TargetAmount - CurrentAmount;
        public string ProgressText => $"{Progress:F1}%";

        public bool IsPrimary => Priority?.Id == Priority.Primary.Id;
        public bool IsSecondary => Priority?.Id == Priority.Secondary.Id;
        public bool IsResidual => Priority?.Id == Priority.Residual.Id;

        public string DisplayName => $"{Name} ({CurrentAmount:N0}/{TargetAmount:N0})";
        public string PriorityName => Priority?.ToString() ?? "Не указан";
    }
}