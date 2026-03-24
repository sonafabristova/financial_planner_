using System;

namespace financial_planner.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public TransactionType Type { get; set; }
        public TransactionCategory Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }

        public string DisplayAmount => Type == TransactionType.Income
            ? $"+{Amount:N0}"
            : $"-{Amount:N0}";

        public string DisplayColor => Type?.Color ?? "#000000";
        public string Icon => Category?.Icon ?? "📝";
    }
}