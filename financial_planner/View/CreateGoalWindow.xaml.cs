using System;
using System.Linq;
using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class CreateGoalWindow : Window
    {
        public CreateGoalWindow()
        {
            InitializeComponent();
        }

        private void PriorityBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ValidatePercentage();
        }

        private void PercentageBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidatePercentage();
        }

        private void ValidatePercentage()
        {
            // Проверяем, что элементы существуют
            if (PercentageBox == null || ErrorText == null || PriorityBox == null)
                return;

            // Проверяем, что текст не пустой
            if (string.IsNullOrWhiteSpace(PercentageBox.Text))
            {
                ErrorText.Visibility = Visibility.Collapsed;
                return;
            }

            if (!int.TryParse(PercentageBox.Text, out int percentage) || percentage < 0 || percentage > 100)
            {
                ErrorText.Text = "Процент должен быть от 0 до 100";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            var selectedItem = PriorityBox.SelectedItem as System.Windows.Controls.ComboBoxItem;
            if (selectedItem == null)
            {
                ErrorText.Visibility = Visibility.Collapsed;
                return;
            }

            string priorityTag = selectedItem.Tag?.ToString();

            Priority priority = null;
            if (priorityTag == "Primary") priority = Priority.Primary;
            else if (priorityTag == "Secondary") priority = Priority.Secondary;
            else priority = Priority.Residual;

            if (priority == null || AppData.CurrentUser == null)
            {
                ErrorText.Visibility = Visibility.Collapsed;
                return;
            }

            var existingGoals = AppData.GetUserGoals(AppData.CurrentUser.Id)
                .Where(g => g.Status == GoalStatus.Active && g.Priority.Id == priority.Id)
                .ToList();

            int totalPercentage = existingGoals.Sum(g => g.AllocationPercentage) + percentage;

            if (totalPercentage > 100)
            {
                ErrorText.Text = $"Сумма процентов для выбранного приоритета превышает 100% (текущая сумма: {totalPercentage - percentage} + {percentage} = {totalPercentage})";
                ErrorText.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorText.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название цели", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TargetAmountBox.Text, out decimal target) || target <= 0)
            {
                MessageBox.Show("Введите корректную целевую сумму", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(PercentageBox.Text, out int percentage) || percentage < 0 || percentage > 100)
            {
                MessageBox.Show("Процент должен быть от 0 до 100", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ErrorText.Visibility == Visibility.Visible)
            {
                MessageBox.Show("Некорректное значение процента", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid()) return;

            var selectedItem = PriorityBox.SelectedItem as System.Windows.Controls.ComboBoxItem;
            string priorityTag = selectedItem?.Tag?.ToString();

            Priority priority = null;
            if (priorityTag == "Primary") priority = Priority.Primary;
            else if (priorityTag == "Secondary") priority = Priority.Secondary;
            else priority = Priority.Residual;

            var goal = new Goal
            {
                UserId = AppData.CurrentUser.Id,
                Name = NameBox.Text.Trim(),
                Description = DescriptionBox.Text.Trim(),
                TargetAmount = decimal.Parse(TargetAmountBox.Text),
                CurrentAmount = 0,
                Priority = priority,
                Status = GoalStatus.Active,
                AllocationPercentage = int.Parse(PercentageBox.Text),
                CreatedDate = DateTime.Now
            };

            AppData.AddGoal(goal);

            MessageBox.Show($"Цель \"{goal.Name}\" успешно создана!", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.UpdateStatistics();
                    mainWindow.LoadGoals();
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
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
        }

        private void ButtonAddExpense_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow();
            addExpenseWindow.ShowDialog();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow mainWindow)
                {
                    mainWindow.UpdateStatistics();
                    mainWindow.LoadGoals();
                }
            }
        }

        private void ButtonGoals_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открыть список всех целей", "Мои цели",
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