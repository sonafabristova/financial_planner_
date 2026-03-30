using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace financial_planner.Models
{
    public static class AppData
    {
        public static User CurrentUser { get; set; }

        public static List<User> Users { get; set; } = new List<User>();
        public static List<Account> Accounts { get; set; } = new List<Account>();
        public static List<Goal> Goals { get; set; } = new List<Goal>();
        public static List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public static decimal TotalIncome { get; set; } = 0;

        private static int _nextUserId = 1;
        private static int _nextAccountId = 1;
        private static int _nextGoalId = 1;
        private static int _nextTransactionId = 1;

        private static string _dataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "FinancialPlanner",
            "data.txt"
        );

        static AppData()
        {
            LoadAllData();
        }

        public static void SaveAllData()
        {
            List<string> lines = new List<string>();

          
            foreach (var user in Users)
            {
                lines.Add($"USER|{user.Id}|{user.Username}|{user.Password}|{user.Email}|{user.FullName}|{user.RegistrationDate}");
            }

        
            foreach (var acc in Accounts)
            {
                lines.Add($"ACC|{acc.Id}|{acc.UserId}|{acc.CurrentBalance}|{acc.MonthlyIncome}|{acc.MonthlyExpenses}|{acc.LastUpdated}");
            }

          
            foreach (var t in Transactions)
            {
                string typeName = t.Type == TransactionType.Income ? "Income" : "Expense";
                string categoryName = t.Category?.Name ?? "";
                lines.Add($"TRANS|{t.Id}|{t.UserId}|{typeName}|{t.Amount}|{categoryName}|{t.Date}|{t.Note}");
            }

            
            lines.Add($"TOTAL|{TotalIncome}");

           
            lines.Add($"IDS|{_nextUserId}|{_nextAccountId}|{_nextGoalId}|{_nextTransactionId}");

            File.WriteAllLines(_dataPath, lines);
        }

        private static void LoadAllData()
        {
            if (File.Exists(_dataPath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(_dataPath);

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] parts = line.Split('|');

                        if (parts[0] == "USER" && parts.Length == 7)
                        {
                            Users.Add(new User
                            {
                                Id = int.Parse(parts[1]),
                                Username = parts[2],
                                Password = parts[3],
                                Email = parts[4],
                                FullName = parts[5],
                                RegistrationDate = DateTime.Parse(parts[6])
                            });
                        }
                        else if (parts[0] == "ACC" && parts.Length == 7)
                        {
                            Accounts.Add(new Account
                            {
                                Id = int.Parse(parts[1]),
                                UserId = int.Parse(parts[2]),
                                CurrentBalance = decimal.Parse(parts[3]),
                                MonthlyIncome = decimal.Parse(parts[4]),
                                MonthlyExpenses = decimal.Parse(parts[5]),
                                LastUpdated = DateTime.Parse(parts[6])
                            });
                        }
                        else if (parts[0] == "TRANS" && parts.Length == 8)
                        {
                            var transaction = new Transaction
                            {
                                Id = int.Parse(parts[1]),
                                UserId = int.Parse(parts[2]),
                                Type = parts[3] == "Income" ? TransactionType.Income : TransactionType.Expense,
                                Amount = decimal.Parse(parts[4]),
                                Date = DateTime.Parse(parts[6]),
                                Note = parts[7]
                            };

                            string categoryName = parts[5];
                            var allCategories = TransactionCategory.GetAll();
                            transaction.Category = allCategories.FirstOrDefault(c => c.Name == categoryName) ?? TransactionCategory.OtherIncome;

                            Transactions.Add(transaction);
                        }
                        else if (parts[0] == "TOTAL" && parts.Length == 2)
                        {
                            TotalIncome = decimal.Parse(parts[1]);
                        }
                        else if (parts[0] == "IDS" && parts.Length == 5)
                        {
                            _nextUserId = int.Parse(parts[1]);
                            _nextAccountId = int.Parse(parts[2]);
                            _nextGoalId = int.Parse(parts[3]);
                            _nextTransactionId = int.Parse(parts[4]);
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Загружено: {Users.Count} пользователей, {Transactions.Count} транзакций");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
                    CreateTestData();
                }
            }
            else
            {
                CreateTestData();
                SaveAllData();
            }
        }

        private static void CreateTestData()
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
                MonthlyIncome = 0,
                MonthlyExpenses = 0,
                LastUpdated = DateTime.Now
            };
            Accounts.Add(testAccount);

            TotalIncome = 80000;

            var incomeTransaction = new Transaction
            {
                Id = _nextTransactionId++,
                UserId = testUser.Id,
                Type = TransactionType.Income,
                Category = TransactionCategory.Salary,
                Amount = 80000,
                Date = DateTime.Now.AddDays(-10),
                Note = "Зарплата за месяц"
            };
            Transactions.Add(incomeTransaction);
            testAccount.CurrentBalance += incomeTransaction.Amount;
            testAccount.MonthlyIncome = TotalIncome;

            var expenseTransaction1 = new Transaction
            {
                Id = _nextTransactionId++,
                UserId = testUser.Id,
                Type = TransactionType.Expense,
                Category = TransactionCategory.Utilities,
                Amount = 15000,
                Date = DateTime.Now.AddDays(-9),
                Note = "Коммунальные услуги"
            };
            Transactions.Add(expenseTransaction1);
            testAccount.CurrentBalance -= expenseTransaction1.Amount;
            testAccount.MonthlyExpenses += expenseTransaction1.Amount;

            var expenseTransaction2 = new Transaction
            {
                Id = _nextTransactionId++,
                UserId = testUser.Id,
                Type = TransactionType.Expense,
                Category = TransactionCategory.Products,
                Amount = 5000,
                Date = DateTime.Now.AddDays(-8),
                Note = "Продукты"
            };
            Transactions.Add(expenseTransaction2);
            testAccount.CurrentBalance -= expenseTransaction2.Amount;
            testAccount.MonthlyExpenses += expenseTransaction2.Amount;

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

            SaveAllData();
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
            SaveAllData();
        }

        public static void AddTransaction(Transaction transaction)
        {
            transaction.Id = _nextTransactionId++;
            Transactions.Add(transaction);

            if (transaction.Type.Id == TransactionType.Income.Id)
            {
                TotalIncome += transaction.Amount;
            }

            var account = GetUserAccount(transaction.UserId);
            if (account != null)
            {
                if (transaction.Type.Id == TransactionType.Income.Id)
                {
                    account.CurrentBalance += transaction.Amount;
                    account.MonthlyIncome = TotalIncome;
                }
                else
                {
                    account.CurrentBalance -= transaction.Amount;
                    account.MonthlyExpenses += transaction.Amount;
                }
                account.LastUpdated = DateTime.Now;
            }

            SaveAllData();
        }
    }
}