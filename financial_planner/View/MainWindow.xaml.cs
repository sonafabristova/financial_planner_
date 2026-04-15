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
            var account = AppData.GetUserAccount(AppData.CurrentUser.Id);
            if (account != null)
            {
                MonthlyIncomeText.Text = $"{account.MonthlyIncome:N0} ₽";
                MonthlyExpensesText.Text = $"{account.MonthlyExpenses:N0} ₽";
            }

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
            MyGoalsWindow myGoalsWindow = new MyGoalsWindow();
            myGoalsWindow.Show();
            this.Close();
        }

        private void ButtonDistribution_Click(object sender, RoutedEventArgs e)
        {
            DistributionWindow distributionWindow = new DistributionWindow();
            bool? result = distributionWindow.ShowDialog();

            if (result == true)
            {
                UpdateStatistics();
                LoadGoals();
            }
        }

        private void ButtonTopUpGoal_Click(object sender, RoutedEventArgs e)
        {
            // Получаем все активные цели пользователя
            var activeGoals = AppData.GetUserGoals(AppData.CurrentUser.Id)
                .Where(g => g.Status.Name == "Активна")
                .ToList();

            if (!activeGoals.Any())
            {
                MessageBox.Show("У вас нет активных целей для пополнения.\nСначала создайте цель.",
                              "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Если есть только одна цель, сразу открываем окно пополнения
            if (activeGoals.Count == 1)
            {
                var topUpWindow = new TopUpGoalWindow(activeGoals.First());
                bool? result = topUpWindow.ShowDialog();

                if (result == true)
                {
                    UpdateStatistics();
                    LoadGoals();
                }
            }
            else
            {
                // Если целей несколько, показываем список для выбора
                var selectionWindow = new GoalSelectionWindow(activeGoals);
                bool? result = selectionWindow.ShowDialog();

                if (result == true && selectionWindow.SelectedGoal != null)
                {
                    var topUpWindow = new TopUpGoalWindow(selectionWindow.SelectedGoal);
                    bool? topUpResult = topUpWindow.ShowDialog();

                    if (topUpResult == true)
                    {
                        UpdateStatistics();
                        LoadGoals();
                    }
                }
            }
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

        private void ButtonNewGoal_Click(object sender, RoutedEventArgs e)
        {
            CreateGoalWindow createGoalWindow = new CreateGoalWindow();
            bool? result = createGoalWindow.ShowDialog();

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