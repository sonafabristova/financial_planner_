using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels;

namespace financial_planner.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void GoalsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as System.Windows.Controls.ListBox;
            if (listBox?.SelectedItem is Goal selectedGoal)
            {
                var viewModel = DataContext as MainViewModel;
                viewModel?.EditGoalCommand.Execute(selectedGoal);
            }
        }
    }
}