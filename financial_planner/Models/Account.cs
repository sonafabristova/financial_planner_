using System;

namespace financial_planner.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public DateTime LastUpdated { get; set; }

        public decimal FreeBalance => MonthlyIncome - MonthlyExpenses;
    }
}