using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;


namespace financial_planner.ViewModels
{
    public class MyGoalsViewModel : ViewModelBase
    {
        private readonly int _userId;
        private ObservableCollection<Goal> _goals;
        private Goal _selectedGoal;
        private int _activeGoalsCount;
        private decimal _totalSaved;
        private decimal _totalTarget;

        public ObservableCollection<Goal> Goals
        {
            get => _goals;
            set => SetProperty(ref _goals, value);
        }

        public Goal SelectedGoal
        {
            get => _selectedGoal;
            set
            {
                SetProperty(ref _selectedGoal, value);
                OnPropertyChanged(nameof(CanTopUp));
            }
        }

        public int ActiveGoalsCount
        {
            get => _activeGoalsCount;
            set => SetProperty(ref _activeGoalsCount, value);
        }

        public decimal TotalSaved
        {
            get => _totalSaved;
            set => SetProperty(ref _totalSaved, value);
        }

        public decimal TotalTarget
        {
            get => _totalTarget;
            set => SetProperty(ref _totalTarget, value);
        }

        public string TotalProgressText => TotalTarget > 0
            ? $"{(double)(TotalSaved / TotalTarget * 100):F1}%"
            : "0%";

        public bool CanTopUp => SelectedGoal != null && SelectedGoal.IsActive;

        // Команды
        public ICommand AddIncomeCommand { get; }
        public ICommand AddExpenseCommand { get; }
        public ICommand NewGoalCommand { get; }
        public ICommand DistributionCommand { get; }
        public ICommand TopUpGoalCommand { get; }
        public ICommand EditGoalCommand { get; }
        public ICommand DeleteGoalCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand HelpCommand { get; }

        public MyGoalsViewModel()
        {
            _userId = AppData.CurrentUser?.Id ?? 0;

            AddIncomeCommand = new RelayCommand(ExecuteAddIncome);
            AddExpenseCommand = new RelayCommand(ExecuteAddExpense);
            NewGoalCommand = new RelayCommand(ExecuteNewGoal);
            DistributionCommand = new RelayCommand(ExecuteDistribution);
            TopUpGoalCommand = new RelayCommand(ExecuteTopUpGoal, p => CanTopUp);
            EditGoalCommand = new RelayCommand(ExecuteEditGoal, p => SelectedGoal != null);  
            DeleteGoalCommand = new RelayCommand(ExecuteDeleteGoal, p => SelectedGoal != null && !SelectedGoal.IsCompleted);
            BackCommand = new RelayCommand(ExecuteBack);
            ExitCommand = new RelayCommand(ExecuteExit);
            HelpCommand = new RelayCommand(ExecuteHelp);

            AppData.DataChanged += OnDataChanged;

            LoadGoals();
        }
        private void OnDataChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LoadGoals();
            });
        }
        public void LoadGoals()
        {
            var allGoals = AppData.GetUserGoals(_userId);
            Goals = new ObservableCollection<Goal>(allGoals);

            ActiveGoalsCount = Goals.Count(g => g.IsActive);
            TotalSaved = Goals.Sum(g => g.CurrentAmount);
            TotalTarget = Goals.Sum(g => g.TargetAmount);

            OnPropertyChanged(nameof(TotalProgressText));
            OnPropertyChanged(nameof(CanTopUp));
        }

        private void ExecuteTopUpGoal(object parameter)
        {
            if (SelectedGoal == null) return;

            var window = new View.TopUpGoalWindow(SelectedGoal);
            if (window.ShowDialog() == true)
            {
                LoadGoals();
                NotifyMainWindowUpdate();
            }
        }
        private void ExecuteEditGoal(object parameter)
        {
            var goal = parameter as Goal ?? SelectedGoal;
            if (goal == null) return;

            var window = new View.GoalCardWindow(goal);
            window.ShowDialog();
        }
        private void ExecuteCreateGoal(object parameter)
        {
            var goal = parameter as Goal ?? SelectedGoal;
            if (goal == null) return;

            var window = new View.CreateGoalWindow();
            if (window.ShowDialog() == true)
            {
                LoadGoals();
                NotifyMainWindowUpdate();
            }
        }

        private void ExecuteDeleteGoal(object parameter)
        {
            if (SelectedGoal == null) return;

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить цель '{SelectedGoal.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AppData.DeleteGoal(SelectedGoal.Id);
                LoadGoals();
                NotifyMainWindowUpdate();

                MessageBox.Show("Цель успешно удалена", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExecuteAddIncome(object parameter)
        {
            var window = new View.AddIncomeWindow();
            if (window.ShowDialog() == true)
            {
                LoadGoals();
                NotifyMainWindowUpdate();
            }
        }

        private void ExecuteAddExpense(object parameter)
        {
            var window = new View.AddExpenseWindow();
            if (window.ShowDialog() == true)
            {
                LoadGoals();
                NotifyMainWindowUpdate();
            }
        }

        private void ExecuteNewGoal(object parameter)
        {
            var window = new View.CreateGoalWindow();
            if (window.ShowDialog() == true)
            {
                LoadGoals();
                NotifyMainWindowUpdate();
            }
        }

        private void ExecuteDistribution(object parameter)
        {
            var window = new View.DistributionWindow();
            if (window.ShowDialog() == true)
            {
                LoadGoals();
                NotifyMainWindowUpdate();
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
            AppData.SaveAllData();
            Application.Current.Shutdown();
        }

        private void ExecuteHelp(object parameter)
        {
            MessageBox.Show(
                "Окно 'Мои цели':\n\n" +
                "• Двойной клик - редактирование цели\n" +
                "• Выберите цель и нажмите 'Внести сумму в накопления' для ручного пополнения\n" +
                "• 'Распределение средств' - автоматическое распределение\n" +
                "• Выполненные цели удалить нельзя",
                "Помощь",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void NotifyMainWindowUpdate()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is View.MainWindow mainWindow)
                {
                    mainWindow.UpdateStatistics();
                    mainWindow.LoadGoals();
                }
            }
        }
    }
}