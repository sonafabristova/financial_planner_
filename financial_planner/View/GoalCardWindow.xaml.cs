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
            LoadData();
        }

        private void LoadData()
        {
            NameText.Text = _goal.Name;
            DescriptionText.Text = string.IsNullOrEmpty(_goal.Description) ? "Нет описания" : _goal.Description;
            PriorityText.Text = _goal.Priority.Name;
            PercentageText.Text = $"{_goal.AllocationPercentage}%";

            // Статус и цвет
            StatusText.Text = _goal.Status.Name;
            if (_goal.Status.Id == 1)
                StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
            else if (_goal.Status.Id == 2)
                StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"));
            else
                StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9E9E9E"));

            // Финансы
            TargetAmountText.Text = $"Целевая сумма: {_goal.TargetAmount:N0} ₽";
            CurrentAmountText.Text = $"Накоплено: {_goal.CurrentAmount:N0} ₽";
            RemainingText.Text = $"Осталось: {(_goal.TargetAmount - _goal.CurrentAmount):N0} ₽";

            // Прогресс
            ProgressBar.Value = _goal.Progress;
            ProgressText.Text = $"{_goal.Progress:F1}%";

            // История
            CreatedDateText.Text = $"Создана: {_goal.CreatedDate:dd.MM.yyyy}";
            CompletedDateText.Text = _goal.CompletedDate.HasValue
                ? $"Выполнена: {_goal.CompletedDate.Value:dd.MM.yyyy}"
                : "Ещё не выполнена";
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}