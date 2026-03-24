using System;
using System.Collections.Generic;
using System.Linq;

namespace financial_planner.Models
{
    public static class AppData
    {
        public static User CurrentUser { get; set; }

        public static List<User> Users { get; set; } = new List<User>();
        public static List<Account> Accounts { get; set; } = new List<Account>();
        public static List<Goal> Goals { get; set; } = new List<Goal>();
        public static List<Transaction> Transactions { get; set; } = new List<Transaction>();

        private static int _nextUserId = 1;
        private static int _nextAccountId = 1;
        private static int _nextGoalId = 1;
        private static int _nextTransactionId = 1;

        static AppData()
        {
            InitializeTestData();
        }

        private static void InitializeTestData()
        {
            var testUser = new User
            {
                Id = _nextUserId++,
                Username = "test",
                Password = "123",
                Email = "test@mail.ru",
                FullName = "Тестовый Пользователь",
                RegistrationDate = DateTime.Now
            };
            Users.Add(testUser);

            var testAccount = new Account
            {
                Id = _nextAccountId++,
                UserId = testUser.Id,
                CurrentBalance = 50000,
                MonthlyIncome = 80000,
                MonthlyExpenses = 30000,
                LastUpdated = DateTime.Now
            };
            Accounts.Add(testAccount);

            var testGoals = new List<Goal>
            {
                new Goal
                {
                    Id = _nextGoalId++,
                    UserId = testUser.Id,
                    Name = "Новый телефон",
                    Description = "iPhone 15",
                    TargetAmount = 90000,
                    CurrentAmount = 25000,
                    Priority = Priority.Primary,
                    Status = GoalStatus.Active,
                    AllocationPercentage = 40,
                    CreatedDate = DateTime.Now.AddDays(-30)
                },
                new Goal
                {
                    Id = _nextGoalId++,
                    UserId = testUser.Id,
                    Name = "Отпуск",
                    Description = "Поездка на море",
                    TargetAmount = 120000,
                    CurrentAmount = 15000,
                    Priority = Priority.Secondary,
                    Status = GoalStatus.Active,
                    AllocationPercentage = 30,
                    CreatedDate = DateTime.Now.AddDays(-15)
                },
                new Goal
                {
                    Id = _nextGoalId++,
                    UserId = testUser.Id,
                    Name = "Новая книга",
                    Description = "Книги по программированию",
                    TargetAmount = 5000,
                    CurrentAmount = 0,
                    Priority = Priority.Residual,
                    Status = GoalStatus.Active,
                    AllocationPercentage = 10,
                    CreatedDate = DateTime.Now.AddDays(-5)
                }
            };

            foreach (var goal in testGoals)
            {
                Goals.Add(goal);
            }

            var testTransactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = _nextTransactionId++,
                    UserId = testUser.Id,
                    Type = TransactionType.Income,
                    Category = TransactionCategory.Salary,
                    Amount = 80000,
                    Date = DateTime.Now.AddDays(-10),
                    Note = "Зарплата за месяц"
                },
                new Transaction
                {
                    Id = _nextTransactionId++,
                    UserId = testUser.Id,
                    Type = TransactionType.Expense,
                    Category = TransactionCategory.Utilities,
                    Amount = 15000,
                    Date = DateTime.Now.AddDays(-9),
                    Note = "Коммунальные услуги"
                },
                new Transaction
                {
                    Id = _nextTransactionId++,
                    UserId = testUser.Id,
                    Type = TransactionType.Expense,
                    Category = TransactionCategory.Products,
                    Amount = 5000,
                    Date = DateTime.Now.AddDays(-8),
                    Note = "Продукты"
                }
            };

            foreach (var transaction in testTransactions)
            {
                Transactions.Add(transaction);
            }
        }

        public static User AuthenticateUser(string username, string password)
        {
            return Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);
        }

        public static bool RegisterUser(string username, string password, string email, string fullName)
        {
            if (Users.Any(u => u.Username == username))
                return false;

            var newUser = new User
            {
                Id = _nextUserId++,
                Username = username,
                Password = password,
                Email = email,
                FullName = fullName,
                RegistrationDate = DateTime.Now
            };
            Users.Add(newUser);

            var newAccount = new Account
            {
                Id = _nextAccountId++,
                UserId = newUser.Id,
                CurrentBalance = 0,
                MonthlyIncome = 0,
                MonthlyExpenses = 0,
                LastUpdated = DateTime.Now
            };
            Accounts.Add(newAccount);

            return true;
        }

        public static Account GetUserAccount(int userId)
        {
            return Accounts.FirstOrDefault(a => a.UserId == userId);
        }

        public static List<Goal> GetUserGoals(int userId)
        {
            return Goals.Where(g => g.UserId == userId).ToList();
        }

        public static void AddGoal(Goal goal)
        {
            goal.Id = _nextGoalId++;
            Goals.Add(goal);
        }

        public static void AddTransaction(Transaction transaction)
        {
            transaction.Id = _nextTransactionId++;
            Transactions.Add(transaction);

            var account = GetUserAccount(transaction.UserId);
            if (account != null)
            {
                if (transaction.Type == TransactionType.Income)
                {
                    account.CurrentBalance += transaction.Amount;
                    account.MonthlyIncome += transaction.Amount;
                }
                else
                {
                    account.CurrentBalance -= transaction.Amount;
                    account.MonthlyExpenses += transaction.Amount;
                }
                account.LastUpdated = DateTime.Now;
            }
        }
    }
}