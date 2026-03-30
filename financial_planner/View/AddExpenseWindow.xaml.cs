using System;
using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class AddExpenseWindow : Window
    {
        public AddExpenseWindow()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму расхода", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var transaction = new Transaction
            {
                UserId = AppData.CurrentUser.Id,
                Type = TransactionType.Expense,
                Amount = amount,
                Category = GetSelectedCategory(),
                Date = DatePicker.SelectedDate ?? DateTime.Now,
                Note = NoteBox.Text
            };

            AppData.AddTransaction(transaction);

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.UpdateStatistics();
                    mainWindow.LoadGoals();
                }
            }

            MessageBox.Show($"Расход в размере {amount:N2} ₽ успешно добавлен", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }

        private TransactionCategory GetSelectedCategory()
        {
            var selectedItem = CategoryBox.SelectedItem as System.Windows.Controls.ComboBoxItem;
            string category = selectedItem?.Content.ToString();

            switch (category)
            {
                case "Продукты":
                    return TransactionCategory.Products;
                case "Транспорт":
                    return TransactionCategory.Transport;
                case "Коммунальные услуги":
                    return TransactionCategory.Utilities;
                case "Развлечения":
                    return TransactionCategory.Entertainment;
                case "Одежда":
                    return new TransactionCategory { Id = 11, Name = "Одежда", Type = TransactionType.Expense, Icon = "👕" };
                case "Здоровье":
                    return new TransactionCategory { Id = 12, Name = "Здоровье", Type = TransactionType.Expense, Icon = "💊" };
                default:
                    return TransactionCategory.OtherExpense;
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Введите сумму расхода.\nВыберите категорию и дату.",
                          "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonAddIncome_Click(object sender, RoutedEventArgs e)
        {
            AddIncomeWindow addIncomeWindow = new AddIncomeWindow();
            addIncomeWindow.ShowDialog();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.UpdateStatistics();
                    mainWindow.LoadGoals();
                }
            }

            this.Close();
        }

        private void ButtonGoals_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открыть список целей", "Мои цели",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonNewGoal_Click(object sender, RoutedEventArgs e)
        {
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
            AppData.SaveAllData();
            Application.Current.Shutdown();
        }
    }
}