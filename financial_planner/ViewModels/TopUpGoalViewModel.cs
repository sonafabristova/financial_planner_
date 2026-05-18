using System;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class TopUpGoalViewModel : ViewModelBase
    {
        private readonly int _userId;
        private Goal _goal;
        private decimal _amount;
        private string _errorMessage;
        private DatabaseService _dbService;

        public Goal Goal
        {
            get => _goal;
            set
            {
                SetProperty(ref _goal, value);
                if (value != null)
                {
                    OnPropertyChanged(nameof(GoalName));
                    OnPropertyChanged(nameof(CurrentAmountText));
                    OnPropertyChanged(nameof(TargetAmountText));
                    OnPropertyChanged(nameof(Progress));
                }
            }
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public string GoalName => Goal?.Name ?? "";
        public string CurrentAmountText => Goal != null ? $"{Goal.CurrentAmount:N0} ₽" : "";
        public string TargetAmountText => Goal != null ? $"{Goal.TargetAmount:N0} ₽" : "";
        public double Progress => Goal?.Progress ?? 0;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand TopUpCommand { get; }
        public ICommand CancelCommand { get; }

        public TopUpGoalViewModel(Goal goal)
        {
            if (AppState.CurrentUser == null)
            {
                throw new InvalidOperationException("Пользователь не авторизован");
            }

            _userId = AppState.CurrentUser.Id;
            _goal = goal ?? throw new ArgumentNullException(nameof(goal));
            _dbService = DatabaseService.Instance;

            TopUpCommand = new RelayCommand(ExecuteTopUp, CanExecuteTopUp);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private bool CanExecuteTopUp(object parameter)
        {
            return Amount > 0 && Goal != null;
        }

        private void ExecuteTopUp(object parameter)
        {
            try
            {
                if (Goal == null)
                {
                    ErrorMessage = "Ошибка: цель не найдена";
                    return;
                }

                var transaction = new Transaction
                {
                    UserId = _userId,
                    CategoryId = 12, // Другой расход
                    Amount = Amount,
                    Date = DateTime.Now,
                    Note = $"Пополнение цели: {Goal.Name}"
                };
                _dbService.AddTransaction(transaction);

                Goal.CurrentAmount += Amount;

                if (Goal.CurrentAmount >= Goal.TargetAmount)
                {
                    Goal.StatusId = 2; // Выполнена
                    Goal.CompletedDate = DateTime.Now;
                }

                _dbService.UpdateGoal(Goal);

                MessageBox.Show($"Цель '{Goal.Name}' пополнена на {Amount:N0} ₽",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                var window = parameter as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        private void ExecuteCancel(object parameter)
        {
            var window = parameter as Window;
            if (window != null)
            {
                window.DialogResult = false;
                window.Close();
            }
        }
    }
}