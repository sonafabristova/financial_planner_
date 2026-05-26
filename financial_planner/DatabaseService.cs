using System;
using System.Collections.Generic;
using System.Linq;
using financial_planner.Models;
using Microsoft.EntityFrameworkCore;

namespace financial_planner
{
    public class DatabaseService
    {
        private static DatabaseService _instance;
        private static readonly object _lock = new object();

        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DatabaseService();
                        }
                    }
                }
                return _instance;
            }
        }

        private DatabaseService() { }

        public static event Action DataChanged;
        public static void NotifyDataChanged() => DataChanged?.Invoke();

        // ==================== Users ====================
        public User AuthenticateUser(string username, string password)
        {
            using (var context = new PlannerContext())
            {
                return context.Users
                    .FirstOrDefault(u => u.Username == username && u.Password == password);
            }
        }

        public bool RegisterUser(User user, Account account)
        {
            using (var context = new PlannerContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Users.Add(user);
                        context.SaveChanges();

                        account.UserId = user.Id;
                        context.Accounts.Add(account);
                        context.SaveChanges();

                        transaction.Commit();
                        NotifyDataChanged();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        // ==================== Accounts ====================
        public Account GetUserAccount(int userId)
        {
            using (var context = new PlannerContext())
            {
                return context.Accounts.FirstOrDefault(a => a.UserId == userId);
            }
        }

        public void UpdateAccount(Account account)
        {
            using (var context = new PlannerContext())
            {
                context.Accounts.Update(account);
                context.SaveChanges();
                NotifyDataChanged();
            }
        }

        // ==================== Transactions ====================
        public void AddTransaction(Transaction transaction)
        {
            using (var context = new PlannerContext())
            {
                context.Transactions.Add(transaction);
                context.SaveChanges();

                var account = GetUserAccount(transaction.UserId);
                if (account != null)
                {
                    var category = context.TransactionCategories.FirstOrDefault(c => c.Id == transaction.CategoryId);
                    if (category != null && category.TypeId == 1)
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
                    UpdateAccount(account);
                }
                NotifyDataChanged();
            }
        }

        public List<Transaction> GetUserTransactions(int userId)
        {
            using (var context = new PlannerContext())
            {
                return context.Transactions
                    .Include(t => t.Category)
                    .ThenInclude(c => c.Type)
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }
        }

        // ==================== Goals ====================
        public List<Goal> GetUserGoals(int userId)
        {
            using (var context = new PlannerContext())
            {
                return context.Goals
                    .Include(g => g.Priority)
                    .Include(g => g.Status)
                    .Where(g => g.UserId == userId)
                    .OrderBy(g => g.PriorityId)
                    .ToList();
            }
        }

        public void AddGoal(Goal goal)
        {
            using (var context = new PlannerContext())
            {
                context.Goals.Add(goal);
                context.SaveChanges();
                NotifyDataChanged();
            }
        }

        public void UpdateGoal(Goal goal)
        {
            using (var context = new PlannerContext())
            {
                var existing = context.Goals.Find(goal.Id);
                if (existing != null)
                {
                    existing.Name = goal.Name;
                    existing.Description = goal.Description;
                    existing.TargetAmount = goal.TargetAmount;
                    existing.CurrentAmount = goal.CurrentAmount;
                    existing.PriorityId = goal.PriorityId;
                    existing.StatusId = goal.StatusId;
                    existing.AllocationPercentage = goal.AllocationPercentage;
                    existing.CompletedDate = goal.CompletedDate;
                    context.SaveChanges();
                }
            }
            NotifyDataChanged();
        }

        public void DeleteGoal(int goalId)
        {
            using (var context = new PlannerContext())
            {
                var goal = context.Goals.Find(goalId);
                if (goal != null)
                {
                    context.Goals.Remove(goal);
                    context.SaveChanges();
                    NotifyDataChanged();
                }
            }
        }

        // ==================== Справочники ====================
        public List<TransactionCategory> GetIncomeCategories()
        {
            using (var context = new PlannerContext())
            {
                return context.TransactionCategories
                    .Include(c => c.Type)
                    .Where(c => c.TypeId == 1)
                    .ToList();
            }
        }

        public List<TransactionCategory> GetExpenseCategories()
        {
            using (var context = new PlannerContext())
            {
                return context.TransactionCategories
                    .Include(c => c.Type)
                    .Where(c => c.TypeId == 2)
                    .ToList();
            }
        }

        public List<Priority> GetPriorities()
        {
            using (var context = new PlannerContext())
            {
                return context.Priorities.ToList();
            }
        }

        public List<GoalStatus> GetGoalStatuses()
        {
            using (var context = new PlannerContext())
            {
                return context.GoalStatuses.ToList();
            }
        }
    }
}