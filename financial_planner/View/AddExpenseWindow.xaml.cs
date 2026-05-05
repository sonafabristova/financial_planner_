using System.Windows;
using financial_planner.Models;
using financial_planner.ViewModels;

namespace financial_planner.View
{
    public partial class AddExpenseWindow : Window
    {
        public AddExpenseWindow()
        {
            InitializeComponent();
        }

        private void ButtonAddIncome_Click(object sender, RoutedEventArgs e)
        {
            var incomeWindow = new AddIncomeWindow();
            incomeWindow.ShowDialog();
            this.Close();
        }

        private void ButtonGoals_Click(object sender, RoutedEventArgs e)
        {
            var goalsWindow = new MyGoalsWindow();
            goalsWindow.Show();
            this.Close();
        }

        private void ButtonNewGoal_Click(object sender, RoutedEventArgs e)
        {
            var createGoalWindow = new CreateGoalWindow();
            createGoalWindow.ShowDialog();
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