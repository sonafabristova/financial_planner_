using System;
using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class AddIncomeWindow : Window
    {
        public AddIncomeWindow()
        {
            InitializeComponent();
        }

        private void AmountBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (decimal.TryParse(AmountBox.Text, out decimal amount))
            {
                decimal tax = amount * 0.13m;
                TaxBox.Text = $"{tax:N2} ₽";
            }
            else
            {
                TaxBox.Text = "0 ₽";
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var transaction = new Transaction
            {
                UserId = AppData.CurrentUser.Id,
                Type = TransactionType.Income,
                Amount = amount,
                Category = GetSelectedCategory(),
                Date = DatePicker.SelectedDate ?? DateTime.Now,
                Note = NoteBox.Text
            };

            AppData.AddTransaction(transaction);

            // Обновляем главное окно
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.UpdateStatistics();
                    mainWindow.LoadGoals();
                }
            }

            MessageBox.Show($"Доход в размере {amount:N2} ₽ успешно добавлен", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }

        private TransactionCategory GetSelectedCategory()
        {
            var selectedItem = SourceBox.SelectedItem as System.Windows.Controls.ComboBoxItem;
            string source = selectedItem?.Content.ToString();

            switch (source)
            {
                case "Зарплата":
                    return TransactionCategory.Salary;
                case "Фриланс":
                    return TransactionCategory.Freelance;
                case "Подарок":
                    return TransactionCategory.Gifts;
                case "Инвестиции":
                    return TransactionCategory.Investments;
                default:
                    return TransactionCategory.OtherIncome;
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Введите сумму дохода.\nНалог рассчитывается автоматически (13%).\nВыберите источник и дату получения дохода.",
                          "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonAddExpense_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Открыть окно внесения расхода
            MessageBox.Show("Открыть окно внесения расхода", "Внести расход",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonGoals_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Открыть окно моих целей
            MessageBox.Show("Открыть список целей", "Мои цели",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonNewGoal_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Открыть окно создания цели
            MessageBox.Show("Открыть окно создания цели", "Новая цель",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}