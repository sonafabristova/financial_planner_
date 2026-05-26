using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class CreateGoalViewModel : ViewModelBase
    {
        private readonly int _userId;
        private string _name;
        private string _description;
        private decimal _targetAmount;
        private Priority _selectedPriority;
        private int _allocationPercentage;
        private string _errorMessage;
        private DatabaseService _dbService;

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
            set => SetProperty(ref _targetAmount, value);
        }

        public Priority SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                if (SetProperty(ref _selectedPriority, value))
                {
                    ValidatePercentage();
                }
            }
        }

        public int AllocationPercentage
        {
            get => _allocationPercentage;
            set
            {
                if (SetProperty(ref _allocationPercentage, value))
                {
                    ValidatePercentage();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public List<Priority> Priorities { get; set; }

        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand HelpCommand { get; }

        public CreateGoalViewModel()
        {
            _userId = AppState.CurrentUser?.Id ?? 0;
            _dbService = DatabaseService.Instance;

            // Загружаем приоритеты из БД
            Priorities = _dbService.GetPriorities();
            SelectedPriority = Priorities.FirstOrDefault();

            CreateCommand = new RelayCommand(ExecuteCreate, CanExecuteCreate);
            CancelCommand = new RelayCommand(ExecuteCancel);
            HelpCommand = new RelayCommand(ExecuteHelp);
        }

        private bool CanExecuteCreate(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   TargetAmount > 0 &&
                   AllocationPercentage >= 0 &&
                   AllocationPercentage <= 100 &&
                   string.IsNullOrEmpty(ErrorMessage);
        }

        private void ValidatePercentage()
        {
            if (SelectedPriority == null) return;
            if (AllocationPercentage < 0 || AllocationPercentage > 100)
            {
                ErrorMessage = "Процент должен быть от 0 до 100";
                return;
            }

            var existingGoals = _dbService.GetUserGoals(_userId)
                .Where(g => g.StatusId == 1 && g.PriorityId == SelectedPriority.Id)
                .ToList();

            int totalPercentage = existingGoals.Sum(g => g.AllocationPercentage) + AllocationPercentage;

            if (totalPercentage > 100)
            {
                ErrorMessage = $"Сумма процентов для выбранного приоритета превышает 100% (всего будет {totalPercentage}%)";
            }
            else
            {
                ErrorMessage = "";
            }
        }

        private void ExecuteCreate(object parameter)
        {
            try
            {
                if (SelectedPriority == null)
                {
                    ErrorMessage = "Выберите приоритет";
                    return;
                }

                var goal = new Goal
                {
                    UserId = _userId,
                    Name = Name.Trim(),
                    Description = Description?.Trim() ?? "",
                    TargetAmount = TargetAmount,
                    CurrentAmount = 0,
                    PriorityId = SelectedPriority.Id,
                    StatusId = 1, // Активна
                    AllocationPercentage = AllocationPercentage,
                    CreatedDate = DateTime.Now,
                    CompletedDate = null
                };

                _dbService.AddGoal(goal);

                MessageBox.Show($"Цель \"{goal.Name}\" успешно создана!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                (parameter as Window)?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        private void ExecuteCancel(object parameter)
        {
            (parameter as Window)?.Close();
        }

        private void ExecuteHelp(object parameter)
        {
            MessageBox.Show("Создание цели накопления:\n\n" +
                "1. Введите название и описание цели\n" +
                "2. Укажите целевую сумму\n" +
                "3. Выберите приоритет:\n" +
                "   - Первичный: деньги распределяются в первую очередь\n" +
                "   - Вторичный: распределяются после первичных\n" +
                "   - Остаточный: распределяются в последнюю очередь\n" +
                "4. Укажите процент от свободных средств\n" +
                "Важно: сумма процентов в одном приоритете не должна превышать 100%",
                "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}