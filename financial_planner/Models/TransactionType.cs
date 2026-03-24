using System.Collections.Generic;

namespace financial_planner.Models
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Color { get; set; }

        public static TransactionType Income => new TransactionType
        {
            Id = 1,
            Name = "Income",
            DisplayName = "Доход",
            Color = "#4CAF50"
        };

        public static TransactionType Expense => new TransactionType
        {
            Id = 2,
            Name = "Expense",
            DisplayName = "Расход",
            Color = "#F44336"
        };

        public static List<TransactionType> GetAll()
        {
            return new List<TransactionType> { Income, Expense };
        }

        public override string ToString() => DisplayName;
    }
}