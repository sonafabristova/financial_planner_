using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly int _userId;
        private ObservableCollection<Goal> _activeGoals;
        private decimal _monthlyIncome;
        private decimal _monthlyExpenses;
        private int _activeGoalsCount;
        private string _userName;

        public ObservableCollection<Goal> ActiveGoals
        {
            get => _activeGoals;
            set => SetProperty(ref _activeGoals, value);
        }

        public decimal MonthlyIncome
        {
            get => _monthlyIncome;
            set => SetProperty(ref _monthlyIncome, value);
        }

        public string MonthlyIncomeText => $"{MonthlyIncome:N0} ₽";

        public decimal MonthlyExpenses
        {
            get => _monthlyExpenses;
            set => SetProperty(ref _monthlyExpenses, value);
        }

        public string MonthlyExpensesText => $"{MonthlyExpenses:N0} ₽";

        public int ActiveGoalsCount
        {
            get => _activeGoalsCount;
            set => SetProperty(ref _activeGoalsCount, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        // Команды
        public ICommand AddIncomeCommand { get; }
        public ICommand AddExpenseCommand { get; }
        public ICommand NewGoalCommand { get; }
        public ICommand DistributionCommand { get; }
        public ICommand TopUpGoalCommand { get; }
        public ICommand GoalsCommand { get; }
        public ICommand HelpCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand EditGoalCommand { get; }
        public ICommand ResetMonthCommand { get; }

        public MainViewModel()
        {
            _userId = AppData.CurrentUser?.Id ?? 0;
            UserName = AppData.CurrentUser?.FullName ?? "Пользователь";

            AddIncomeCommand = new RelayCommand(ExecuteAddIncome);
            AddExpenseCommand = new RelayCommand(ExecuteAddExpense);
            NewGoalCommand = new RelayCommand(ExecuteNewGoal);
            DistributionCommand = new RelayCommand(ExecuteDistribution);
            TopUpGoalCommand = new RelayCommand(ExecuteTopUpGoal);
            GoalsCommand = new RelayCommand(ExecuteGoals);
            HelpCommand = new RelayCommand(ExecuteHelp);
            ExitCommand = new RelayCommand(ExecuteExit);
            EditGoalCommand = new RelayCommand(ExecuteEditGoal);
            ResetMonthCommand = new RelayCommand(ExecuteResetMonth);

            AppData.DataChanged += OnDataChanged;

            LoadData();
        }
        private void OnDataChanged()
        {
            // Обновляем данные в UI потоке
            Application.Current.Dispatcher.Invoke(() =>
            {
                LoadData();
            });
        }
        public void LoadData()
        {
            LoadStatistics();
            LoadActiveGoals();
            // Принудительно обновляем UI
            OnPropertyChanged(nameof(MonthlyIncomeText));
            OnPropertyChanged(nameof(MonthlyExpensesText));
            OnPropertyChanged(nameof(ActiveGoalsCount));
        }

        private void LoadStatistics()
        {
            var account = AppData.GetUserAccount(_userId);
            if (account != null)
            {
                MonthlyIncome = account.MonthlyIncome;
                MonthlyExpenses = account.MonthlyExpenses;
            }
        }

        private void LoadActiveGoals()
        {
            var goals = AppData.GetUserGoals(_userId)
                .Where(g => g.IsActive)
                .ToList();
            ActiveGoals = new ObservableCollection<Goal>(goals);
            ActiveGoalsCount = ActiveGoals.Count;
        }
        private void ExecuteAddIncome(object parameter)
        {
            var window = new View.AddIncomeWindow();
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void ExecuteAddExpense(object parameter)
        {
            var window = new View.AddExpenseWindow();
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void ExecuteNewGoal(object parameter)
        {
            var window = new View.CreateGoalWindow();
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }
        private void ExecuteResetMonth(object parameter)
        {
            var account = AppData.GetUserAccount(_userId);
            if (account != null)
            {
                account.MonthlyIncome = 0;
                account.MonthlyExpenses = 0;
                AppData.SaveAllData();
                LoadStatistics();

                // Принудительно обновляем UI
                OnPropertyChanged(nameof(MonthlyIncomeText));
                OnPropertyChanged(nameof(MonthlyExpensesText));

                MessageBox.Show("Статистика за месяц сброшена", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ExecuteDistribution(object parameter)
        {
            var window = new View.DistributionWindow();
            if (window.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void ExecuteTopUpGoal(object parameter)
        {
            var activeGoals = AppData.GetUserGoals(_userId)
                .Where(g => g.IsActive)
                .ToList();

            if (!activeGoals.Any())
            {
                MessageBox.Show("У вас нет активных целей для пополнения.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (activeGoals.Count == 1)
            {
                var window = new View.TopUpGoalWindow(activeGoals.First());
                if (window.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                var selectionWindow = new View.GoalSelectionWindow(activeGoals);
                if (selectionWindow.ShowDialog() == true && selectionWindow.SelectedGoal != null)
                {
                    var window = new View.TopUpGoalWindow(selectionWindow.SelectedGoal);
                    if (window.ShowDialog() == true)
                    {
                        LoadData();
                    }
                }
            }
        }

        private void ExecuteGoals(object parameter)
        {
            var window = new View.MyGoalsWindow();
            window.Show();
            Application.Current.Windows.OfType<View.MainWindow>().FirstOrDefault()?.Close();
        }

        private void ExecuteHelp(object parameter)
        {
            MessageBox.Show(
                "Помощь по программе:\n\n" +
                "1. Вносите доходы и расходы\n" +
                "2. Создавайте цели накопления\n" +
                "3. Используйте 'Распределение средств' для автоматического накопления\n" +
                "4. Отслеживайте прогресс в списке целей\n" +
                "5. Двойной клик по цели - подробная карточка",
                "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteExit(object parameter)
        {
            AppData.SaveAllData();
            Application.Current.Shutdown();
        }

        private void ExecuteEditGoal(object parameter)
        {
            if (parameter is Goal goal)
            {
                var window = new View.GoalCardWindow(goal);
                window.ShowDialog();
            }
        }
        public void Cleanup()
        {
            AppData.DataChanged -= OnDataChanged;
        }
    }
}