using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels;

namespace financial_planner.View
{
    public partial class MyGoalsWindow : Window
    {
        public MyGoalsWindow()
        {
            InitializeComponent();
            DataContext = new MyGoalsViewModel();
        }

        private void GoalsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GoalsListBox.SelectedItem is Goal selectedGoal)
            {
                var goalCardWindow = new GoalCardWindow(selectedGoal);
                goalCardWindow.ShowDialog();
            }
        }
    }
}