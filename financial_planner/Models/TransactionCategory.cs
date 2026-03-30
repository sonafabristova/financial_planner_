using System;
using System.Collections.Generic;
using System.Linq;

namespace financial_planner.Models
{
    [Serializable]
    public class TransactionCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TransactionType Type { get; set; }
        public string Icon { get; set; }

        public static TransactionCategory Salary => new TransactionCategory
        {
            Id = 1,
            Name = "Зарплата",
            Type = TransactionType.Income,
            Icon = "💰"
        };

        public static TransactionCategory Freelance => new TransactionCategory
        {
            Id = 2,
            Name = "Фриланс",
            Type = TransactionType.Income,
            Icon = "💻"
        };

        public static TransactionCategory Gifts => new TransactionCategory
        {
            Id = 3,
            Name = "Подарки",
            Type = TransactionType.Income,
            Icon = "🎁"
        };

        public static TransactionCategory Investments => new TransactionCategory
        {
            Id = 4,
            Name = "Инвестиции",
            Type = TransactionType.Income,
            Icon = "📈"
        };

        public static TransactionCategory OtherIncome => new TransactionCategory
        {
            Id = 5,
            Name = "Другой доход",
            Type = TransactionType.Income,
            Icon = "📥"
        };

        public static TransactionCategory Products => new TransactionCategory
        {
            Id = 6,
            Name = "Продукты",
            Type = TransactionType.Expense,
            Icon = "🛒"
        };

        public static TransactionCategory Transport => new TransactionCategory
        {
            Id = 7,
            Name = "Транспорт",
            Type = TransactionType.Expense,
            Icon = "🚗"
        };

        public static TransactionCategory Utilities => new TransactionCategory
        {
            Id = 8,
            Name = "Коммунальные услуги",
            Type = TransactionType.Expense,
            Icon = "🏠"
        };

        public static TransactionCategory Entertainment => new TransactionCategory
        {
            Id = 9,
            Name = "Развлечения",
            Type = TransactionType.Expense,
            Icon = "🎬"
        };

        public static TransactionCategory OtherExpense => new TransactionCategory
        {
            Id = 10,
            Name = "Другой расход",
            Type = TransactionType.Expense,
            Icon = "📤"
        };

        public static List<TransactionCategory> GetAll()
        {
            return new List<TransactionCategory>
            {
                Salary, Freelance, Gifts, Investments, OtherIncome,
                Products, Transport, Utilities, Entertainment, OtherExpense
            };
        }

        public static List<TransactionCategory> GetIncomeCategories()
        {
            return GetAll().Where(c => c.Type == TransactionType.Income).ToList();
        }

        public static List<TransactionCategory> GetExpenseCategories()
        {
            return GetAll().Where(c => c.Type == TransactionType.Expense).ToList();
        }

        public override string ToString() => Name;
    }
}