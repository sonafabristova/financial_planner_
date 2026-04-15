using System;
using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class TopUpGoalWindow : Window
    {
        private Goal _goal;

        public TopUpGoalWindow(Goal goal)
        {
            InitializeComponent();
            _goal = goal;

            GoalNameText.Text = _goal.Name;
            CurrentAmountText.Text = $"{_goal.CurrentAmount:N0} ₽";
            TargetAmountText.Text = $"{_goal.TargetAmount:N0} ₽";
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Добавляем транзакцию пополнения цели (как расход)
            var transaction = new Transaction
            {
                UserId = AppData.CurrentUser.Id,
                Type = TransactionType.Expense,
                Amount = amount,
                Category = TransactionCategory.OtherExpense,
                Date = DateTime.Now,
                Note = $"Пополнение цели: {_goal.Name}"
            };
            AppData.AddTransaction(transaction);

            // Обновляем сумму цели
            _goal.CurrentAmount += amount;

            // Если цель достигнута, меняем статус
            if (_goal.CurrentAmount >= _goal.TargetAmount)
            {
                _goal.Status = GoalStatus.Completed;
                _goal.CompletedDate = DateTime.Now;
            }

            AppData.UpdateGoal(_goal);

            MessageBox.Show($"Цель '{_goal.Name}' пополнена на {amount:N0} ₽", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}