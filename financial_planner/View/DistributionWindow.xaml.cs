using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class DistributionWindow : Window
    {
        private List<DistributionItem> _distributionItems;

        public DistributionWindow()
        {
            InitializeComponent();
            CalculateDistribution();
        }

        private void CalculateDistribution()
        {
            var account = AppData.GetUserAccount(AppData.CurrentUser.Id);
            if (account == null)
            {
                FreeBalanceText.Text = "0 ₽";
                return;
            }

            decimal freeBalance = account.FreeBalance;
            FreeBalanceText.Text = $"{freeBalance:N0} ₽";

            if (freeBalance <= 0)
            {
                DistributionList.ItemsSource = new List<DistributionItem>();
                ApplyButton.IsEnabled = false;
                return;
            }

            var activeGoals = AppData.GetUserGoals(AppData.CurrentUser.Id)
                .Where(g => g.IsActive && g.Remaining > 0)
                .ToList();

            var distribution = new List<DistributionItem>();
            decimal remainingBalance = freeBalance;

            var primaryGoals = activeGoals.Where(g => g.IsPrimary).ToList();
            remainingBalance = DistributeByPriority(primaryGoals, remainingBalance, distribution);

            var secondaryGoals = activeGoals.Where(g => g.IsSecondary).ToList();
            remainingBalance = DistributeByPriority(secondaryGoals, remainingBalance, distribution);

            var residualGoals = activeGoals.Where(g => g.IsResidual).ToList();
            remainingBalance = DistributeByPriority(residualGoals, remainingBalance, distribution);

            _distributionItems = distribution;
            DistributionList.ItemsSource = _distributionItems;
        }

        private decimal DistributeByPriority(List<Goal> goals, decimal availableAmount, List<DistributionItem> distribution)
        {
            if (!goals.Any() || availableAmount <= 0)
                return availableAmount;

            int totalPercentage = goals.Sum(g => g.AllocationPercentage);

            if (totalPercentage == 0)
                return availableAmount;

            foreach (var goal in goals)
            {
                decimal suggestedAmount = availableAmount * goal.AllocationPercentage / 100;

                if (suggestedAmount > goal.Remaining)
                    suggestedAmount = goal.Remaining;

                distribution.Add(new DistributionItem
                {
                    GoalId = goal.Id,
                    GoalName = goal.Name,
                    PriorityName = goal.Priority.Name,
                    Percentage = goal.AllocationPercentage,
                    SuggestedAmount = suggestedAmount
                });
            }

            return availableAmount - distribution.Where(d => goals.Any(g => g.Id == d.GoalId)).Sum(d => d.SuggestedAmount);
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_distributionItems == null || !_distributionItems.Any())
                {
                    MessageBox.Show("Нет средств для распределения", "Внимание",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                decimal totalAmount = _distributionItems.Sum(d => d.SuggestedAmount);

                if (totalAmount > 0)
                {
                    var transaction = new Transaction
                    {
                        UserId = AppData.CurrentUser.Id,
                        Type = TransactionType.Expense,
                        Amount = totalAmount,
                        Category = TransactionCategory.OtherExpense,
                        Date = DateTime.Now,
                        Note = "Автораспределение средств по целям"
                    };
                    AppData.AddTransaction(transaction);
                }

                foreach (var item in _distributionItems)
                {
                    var goal = AppData.Goals.FirstOrDefault(g => g.Id == item.GoalId);
                    if (goal != null && item.SuggestedAmount > 0)
                    {
                        goal.CurrentAmount += item.SuggestedAmount;

                        if (goal.CurrentAmount >= goal.TargetAmount)
                        {
                            goal.Status = GoalStatus.Completed;
                            goal.CompletedDate = DateTime.Now;
                        }

                        AppData.UpdateGoal(goal);
                    }
                }

                MessageBox.Show($"Средства успешно распределены!\nВсего направлено на накопления: {totalAmount:N0} ₽",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Закрываем окно с результатом
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Окно 'Распределение средств':\n\n" +
                "• Программа автоматически распределяет свободные средства\n" +
                "• Сначала заполняются цели с приоритетом 'Первичный'\n" +
                "• Затем 'Вторичный', потом 'Остаточный'\n" +
                "• Проценты распределения задаются при создании цели\n" +
                "• Если согласны с предложением, нажмите 'Внести суммы в накопления'",
                "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonAddIncome_Click(object sender, RoutedEventArgs e)
        {
            var addIncomeWindow = new AddIncomeWindow();
            addIncomeWindow.ShowDialog();
            CalculateDistribution();
        }

        private void ButtonAddExpense_Click(object sender, RoutedEventArgs e)
        {
            var addExpenseWindow = new AddExpenseWindow();
            addExpenseWindow.ShowDialog();
            CalculateDistribution();
        }

        private void ButtonGoals_Click(object sender, RoutedEventArgs e)
        {
            var myGoalsWindow = new MyGoalsWindow();
            myGoalsWindow.Show();
            Close();
        }

        private void ButtonNewGoal_Click(object sender, RoutedEventArgs e)
        {
            var createGoalWindow = new CreateGoalWindow();
            createGoalWindow.ShowDialog();
            CalculateDistribution();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            AppData.SaveAllData();
            Application.Current.Shutdown();
        }
    }

    public class DistributionItem
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; }
        public string PriorityName { get; set; }
        public int Percentage { get; set; }
        public decimal SuggestedAmount { get; set; }
    }
}