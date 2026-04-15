using System;
using System.Windows;
using System.Windows.Media;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class GoalCardWindow : Window
    {
        private Goal _goal;

        public GoalCardWindow(Goal goal)
        {
            InitializeComponent();
            _goal = goal;
            LoadGoalData();
        }

        private void LoadGoalData()
        {
            // Основная информация
            NameText.Text = _goal.Name;
            DescriptionText.Text = string.IsNullOrEmpty(_goal.Description) ? "Нет описания" : _goal.Description;
            PriorityText.Text = _goal.Priority.Name;
            PercentageText.Text = $"{_goal.AllocationPercentage}%";

            // Статус с цветом
            StatusText.Text = _goal.Status.Name;
            switch (_goal.Status.Id)
            {
                case 1: // Активна
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                    break;
                case 2: // Выполнена
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"));
                    break;
                case 3: // Архивирована
                    StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E9E9E"));
                    break;
            }

            // Финансовая информация
            TargetAmountText.Text = $"{_goal.TargetAmount:N0} ₽";
            CurrentAmountText.Text = $"{_goal.CurrentAmount:N0} ₽";
            RemainingText.Text = $"{_goal.Remaining:N0} ₽";

            // Прогресс
            ProgressBar.Value = _goal.Progress;
            ProgressText.Text = $"{_goal.Progress:F1}%";

            // История
            CreatedDateText.Text = $"Создана: {_goal.CreatedDate:dd.MM.yyyy HH:mm}";
            if (_goal.CompletedDate.HasValue)
                CompletedDateText.Text = $"Выполнена: {_goal.CompletedDate.Value:dd.MM.yyyy HH:mm}";
            else
                CompletedDateText.Text = "Ещё не выполнена";
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}