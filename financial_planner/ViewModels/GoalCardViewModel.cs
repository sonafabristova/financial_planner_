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
        private string _name;
        private string _description;
        private decimal _targetAmount;
        private decimal _currentAmount;
        private Priority _selectedPriority;
        private GoalStatus _selectedStatus;
        private int _allocationPercentage;
        private string _errorMessage;

        public Goal Goal => _goal;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public decimal TargetAmount
        {
            get => _targetAmount;
            set
            {
                if (SetProperty(ref _targetAmount, value))
                {
                    OnPropertyChanged(nameof(TargetAmountText));
                    OnPropertyChanged(nameof(RemainingText));
                    OnPropertyChanged(nameof(Progress));
                    OnPropertyChanged(nameof(ProgressText));
                }
            }
        }

        public string TargetAmountText => $"{TargetAmount:N0} ₽";

        public decimal CurrentAmount
        {
            get => _currentAmount;
            set
            {
                if (SetProperty(ref _currentAmount, value))
                {
                    OnPropertyChanged(nameof(CurrentAmountText));
                    OnPropertyChanged(nameof(RemainingText));
                    OnPropertyChanged(nameof(Progress));
                    OnPropertyChanged(nameof(ProgressText));
                }
            }
        }

        public string CurrentAmountText => $"{CurrentAmount:N0} ₽";

        public decimal Remaining => TargetAmount - CurrentAmount;
        public string RemainingText => $"{Remaining:N0} ₽";

        public double Progress => TargetAmount > 0 ? (double)(CurrentAmount / TargetAmount * 100) : 0;
        public string ProgressText => $"{Progress:F1}%";

        public Priority SelectedPriority
        {
            get => _selectedPriority;
            set => SetProperty(ref _selectedPriority, value);
        }

        public GoalStatus SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public int AllocationPercentage
        {
            get => _allocationPercentage;
            set => SetProperty(ref _allocationPercentage, value);
        }

        public string PercentageText => $"{AllocationPercentage}%";

        public string PriorityName => SelectedPriority?.Name ?? "";
        public string StatusName => SelectedStatus?.Name ?? "";

        public string StatusColor
        {
            get
            {
                if (SelectedStatus?.Id == 1) return "#4CAF50";
                if (SelectedStatus?.Id == 2) return "#2196F3";
                return "#9E9E9E";
            }
        }

        public string CreatedDateText => Goal?.CreatedDate.ToString("dd.MM.yyyy HH:mm") ?? "";
        public string CompletedDateText => Goal?.CompletedDate?.ToString("dd.MM.yyyy HH:mm") ?? "Ещё не выполнена";

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public List<Priority> Priorities { get; }
        public List<GoalStatus> Statuses { get; }

        public GoalCardViewModel(Goal goal)
        {
            _goal = goal;
            Priorities = Priority.GetAll();
            Statuses = GoalStatus.GetAll();

            LoadGoalData();
        }

        private void LoadGoalData()
        {
            Name = Goal.Name;
            Description = Goal.Description ?? "";
            TargetAmount = Goal.TargetAmount;
            CurrentAmount = Goal.CurrentAmount;
            SelectedPriority = Goal.Priority;
            SelectedStatus = Goal.Status;
            AllocationPercentage = Goal.AllocationPercentage;
        }

        public bool Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    ErrorMessage = "Введите название цели";
                    return false;
                }

                if (TargetAmount <= 0)
                {
                    ErrorMessage = "Целевая сумма должна быть больше 0";
                    return false;
                }

                if (AllocationPercentage < 0 || AllocationPercentage > 100)
                {
                    ErrorMessage = "Процент должен быть от 0 до 100";
                    return false;
                }

                Goal.Name = Name.Trim();
                Goal.Description = Description?.Trim() ?? "";
                Goal.TargetAmount = TargetAmount;
                Goal.CurrentAmount = CurrentAmount;
                Goal.Priority = SelectedPriority;
                Goal.Status = SelectedStatus;
                Goal.AllocationPercentage = AllocationPercentage;

                if (SelectedStatus?.Id == 2 && Goal.CompletedDate == null)
                    Goal.CompletedDate = DateTime.Now;
                else if (SelectedStatus?.Id != 2)
                    Goal.CompletedDate = null;

                AppData.UpdateGoal(Goal);

                MessageBox.Show("Цель успешно обновлена", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                ErrorMessage = "";
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                return false;
            }
        }

        public void CancelEdit()
        {
            LoadGoalData();
            ErrorMessage = "";
        }
    }
}