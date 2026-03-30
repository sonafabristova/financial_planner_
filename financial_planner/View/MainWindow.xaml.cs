using System;
using System.Linq;
using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (AppData.CurrentUser != null)
            {
                UserNameRun.Text = AppData.CurrentUser.FullName;

                UpdateStatistics();
                LoadGoals();
            }
        }

        public void UpdateStatistics()
        {
            MonthlyIncomeText.Text = $"{AppData.TotalIncome:N0} ₽";

            var activeGoals = AppData.GetUserGoals(AppData.CurrentUser.Id)
                .Where(g => g.Status.Name == "Активна")
                .Count();
            ActiveGoalsCountText.Text = activeGoals.ToString();
        }

        public void LoadGoals()
        {
            var goals = AppData.GetUserGoals(AppData.CurrentUser.Id)
                .Where(g => g.Status.Name == "Активна")
                .ToList();

            GoalsList.ItemsSource = goals;
        }

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Помощь по программе:\n\n" +
                "1. Вносите доходы и расходы\n" +
                "2. Создавайте цели накопления\n" +
                "3. Используйте 'Распределение средств' для автоматического накопления\n" +
                "4. Отслеживайте прогресс в списке целей",
                "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonGoals_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открыть список всех целей", "Мои цели",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonNewGoal_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открыть окно создания новой цели", "Новая цель",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonDistribution_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открыть окно распределения средств", "Распределение",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonTopUpGoal_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Открыть окно пополнения цели", "Пополнить цель",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ButtonAddIncome_Click(object sender, RoutedEventArgs e)
        {
            AddIncomeWindow addIncomeWindow = new AddIncomeWindow();
            bool? result = addIncomeWindow.ShowDialog();

            if (result == true)
            {
                UpdateStatistics();
                LoadGoals();
            }
        }

        private void ButtonAddExpense_Click(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow();
            bool? result = addExpenseWindow.ShowDialog();

            if (result == true)
            {
                UpdateStatistics();
                LoadGoals();
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            AppData.SaveAllData();
            Application.Current.Shutdown();
        }
    }
}