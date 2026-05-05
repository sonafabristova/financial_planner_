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

        private void ButtonAddIncome_Click(object sender, RoutedEventArgs e)
        {
            var incomeWindow = new AddIncomeWindow();
            incomeWindow.ShowDialog();
        }

        private void ButtonAddExpense_Click(object sender, RoutedEventArgs e)
        {
            var expenseWindow = new AddExpenseWindow();
            expenseWindow.ShowDialog();
        }

        private void ButtonGoals_Click(object sender, RoutedEventArgs e)
        {
            var goalsWindow = new MyGoalsWindow();
            goalsWindow.Show();
            this.Close();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
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