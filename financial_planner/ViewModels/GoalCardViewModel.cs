using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class GoalCardViewModel : ViewModelBase
    {
        private Goal _goal;
        private DatabaseService _dbService;
        private string _errorMessage;

        public Goal Goal => _goal;

        public string GoalName => _goal?.Name ?? "";
        public string GoalDescription => _goal?.Description ?? "Нет описания";
        public string PriorityName => _goal?.Priority?.Name ?? "";
        public string StatusName => _goal?.Status?.Name ?? "";
        public string PercentageText => $"{_goal?.AllocationPercentage ?? 0}%";

        public string StatusColor
        {
            get
            {
                if (_goal?.StatusId == 1) return "#4CAF50";
                if (_goal?.StatusId == 2) return "#2196F3";
                return "#9E9E9E";
            }
        }

        public string TargetAmountText => $"{_goal?.TargetAmount:N0} ₽";
        public string CurrentAmountText => $"{_goal?.CurrentAmount:N0} ₽";
        public string RemainingText => $"{(_goal?.TargetAmount - _goal?.CurrentAmount ?? 0):N0} ₽";
        public double Progress => _goal?.Progress ?? 0;
        public string ProgressText => $"{Progress:F1}%";

        public string CreatedDateText => _goal?.CreatedDate.ToString("dd.MM.yyyy") ?? "";
        public string CompletedDateText => _goal?.CompletedDate?.ToString("dd.MM.yyyy") ?? "Ещё не выполнена";

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand EditCommand { get; }
        public ICommand CloseCommand { get; }

        public GoalCardViewModel(Goal goal)
        {
            _goal = goal;
            _dbService = DatabaseService.Instance;

            EditCommand = new RelayCommand(ExecuteEdit);
            CloseCommand = new RelayCommand(ExecuteClose);
        }

        private void ExecuteEdit(object parameter)
        {
            // TODO: EditGoalWindow будет добавлен позже
            MessageBox.Show("Редактирование цели будет доступно в следующей версии", "Информация",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            // var editWindow = new View.EditGoalWindow(_goal);
            // if (editWindow.ShowDialog() == true)
            // {
            //     _goal = _dbService.GetUserGoals(_goal.UserId).FirstOrDefault(g => g.Id == _goal.Id);
            //     if (_goal != null)
            //     {
            //         OnPropertyChanged(nameof(GoalName));
            //         OnPropertyChanged(nameof(GoalDescription));
            //         OnPropertyChanged(nameof(PriorityName));
            //         OnPropertyChanged(nameof(StatusName));
            //         OnPropertyChanged(nameof(StatusColor));
            //         OnPropertyChanged(nameof(PercentageText));
            //         OnPropertyChanged(nameof(TargetAmountText));
            //         OnPropertyChanged(nameof(CurrentAmountText));
            //         OnPropertyChanged(nameof(RemainingText));
            //         OnPropertyChanged(nameof(Progress));
            //         OnPropertyChanged(nameof(ProgressText));
            //         OnPropertyChanged(nameof(CompletedDateText));
            //     }
            // }
        }

        private void ExecuteClose(object parameter)
        {
            (parameter as Window)?.Close();
        }
    }
}