using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class DistributionItem
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; }
        public string PriorityName { get; set; }
        public int Percentage { get; set; }
        public decimal SuggestedAmount { get; set; }
        public decimal Remaining { get; set; }
    }

    public class DistributionViewModel : ViewModelBase
    {
        private readonly int _userId;
        private ObservableCollection<DistributionItem> _distributionItems;
        private string _freeBalanceText;
        private bool _canApply;
        private DatabaseService _dbService;

        public ObservableCollection<DistributionItem> DistributionItems
        {
            get => _distributionItems;
            set => SetProperty(ref _distributionItems, value);
        }

        public string FreeBalanceText
        {
            get => _freeBalanceText;
            set => SetProperty(ref _freeBalanceText, value);
        }

        public bool CanApply
        {
            get => _canApply;
            set => SetProperty(ref _canApply, value);
        }

        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand HelpCommand { get; }
        public ICommand AddIncomeCommand { get; }
        public ICommand AddExpenseCommand { get; }
        public ICommand GoalsCommand { get; }
        public ICommand NewGoalCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ExitCommand { get; }

        public DistributionViewModel()
        {
            _userId = AppState.CurrentUser?.Id ?? 0;
            _dbService = DatabaseService.Instance;

            ApplyCommand = new RelayCommand(o => ExecuteApply(o), o => CanApply);
            CancelCommand = new RelayCommand(ExecuteCancel);
            HelpCommand = new RelayCommand(ExecuteHelp);
            AddIncomeCommand = new RelayCommand(ExecuteAddIncome);
            AddExpenseCommand = new RelayCommand(ExecuteAddExpense);
            GoalsCommand = new RelayCommand(ExecuteGoals);
            NewGoalCommand = new RelayCommand(ExecuteNewGoal);
            BackCommand = new RelayCommand(ExecuteBack);
            ExitCommand = new RelayCommand(ExecuteExit);

            CalculateDistribution();
        }

        public void CalculateDistribution()
        {
            var account = _dbService.GetUserAccount(_userId);
            if (account == null)
            {
                FreeBalanceText = "0 ₽";
                CanApply = false;
                DistributionItems = new ObservableCollection<DistributionItem>();
                return;
            }

            decimal freeBalance = account.MonthlyIncome - account.MonthlyExpenses;
            FreeBalanceText = $"{freeBalance:N0} ₽";

            if (freeBalance <= 0)
            {
                DistributionItems = new ObservableCollection<DistributionItem>();
                CanApply = false;
                return;
            }

            var activeGoals = _dbService.GetUserGoals(_userId)
                .Where(g => g.StatusId == 1 && g.Remaining > 0)
                .ToList();

            var distribution = new ObservableCollection<DistributionItem>();
            decimal remainingBalance = freeBalance;

            var primaryGoals = activeGoals.Where(g => g.PriorityId == 1).ToList();
            remainingBalance = DistributeByPriority(primaryGoals, remainingBalance, distribution);

            var secondaryGoals = activeGoals.Where(g => g.PriorityId == 2).ToList();
            remainingBalance = DistributeByPriority(secondaryGoals, remainingBalance, distribution);

            var residualGoals = activeGoals.Where(g => g.PriorityId == 3).ToList();
            remainingBalance = DistributeByPriority(residualGoals, remainingBalance, distribution);

            DistributionItems = distribution;
            CanApply = distribution.Any(d => d.SuggestedAmount > 0);
        }

        private decimal DistributeByPriority(System.Collections.Generic.List<Goal> goals, decimal availableAmount, ObservableCollection<DistributionItem> distribution)
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
                    SuggestedAmount = suggestedAmount,
                    Remaining = goal.Remaining
                });
            }

            return availableAmount - distribution.Where(d => goals.Any(g => g.Id == d.GoalId)).Sum(d => d.SuggestedAmount);
        }

        private void ExecuteApply(object parameter)
        {
            if (DistributionItems == null || !DistributionItems.Any())
            {
                MessageBox.Show("Нет средств для распределения", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal totalAmount = DistributionItems.Sum(d => d.SuggestedAmount);

            if (totalAmount > 0)
            {
                var transaction = new Transaction
                {
                    UserId = _userId,
                    CategoryId = 12, // Другой расход
                    Amount = totalAmount,
                    Date = DateTime.Now,
                    Note = "Автораспределение средств по целям"
                };
                _dbService.AddTransaction(transaction);
            }

            foreach (var item in DistributionItems)
            {
                var goal = _dbService.GetUserGoals(_userId).FirstOrDefault(g => g.Id == item.GoalId);
                if (goal != null && item.SuggestedAmount > 0)
                {
                    goal.CurrentAmount += item.SuggestedAmount;

                    if (goal.CurrentAmount >= goal.TargetAmount)
                    {
                        goal.StatusId = 2; // Выполнена
                        goal.CompletedDate = DateTime.Now;
                    }

                    _dbService.UpdateGoal(goal);
                }
            }

            MessageBox.Show($"Средства успешно распределены!\nВсего направлено: {totalAmount:N0} ₽",
                          "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            (parameter as Window)?.Close();
        }

        private void ExecuteCancel(object parameter)
        {
            (parameter as Window)?.Close();
        }

        private void ExecuteHelp(object parameter)
        {
            MessageBox.Show("Окно 'Распределение средств':\n\n" +
                "• Программа автоматически распределяет свободные средства\n" +
                "• Сначала заполняются цели с приоритетом 'Первичный'\n" +
                "• Затем 'Вторичный', потом 'Остаточный'\n" +
                "• Проценты распределения задаются при создании цели",
                "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteAddIncome(object parameter)
        {
            var window = new View.AddIncomeWindow();
            if (window.ShowDialog() == true)
            {
                CalculateDistribution();
            }
        }

        private void ExecuteAddExpense(object parameter)
        {
            var window = new View.AddExpenseWindow();
            if (window.ShowDialog() == true)
            {
                CalculateDistribution();
            }
        }

        private void ExecuteGoals(object parameter)
        {
            var window = new View.MyGoalsWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private void ExecuteNewGoal(object parameter)
        {
            var window = new View.CreateGoalWindow();
            if (window.ShowDialog() == true)
            {
                CalculateDistribution();
            }
        }

        private void ExecuteBack(object parameter)
        {
            var mainWindow = new View.MainWindow();
            mainWindow.Show();
            (parameter as Window)?.Close();
        }

        private void ExecuteExit(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}