using System.Collections.Generic;
using System.Linq;
using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class GoalSelectionWindow : Window
    {
        public Goal SelectedGoal { get; private set; }

        public GoalSelectionWindow(List<Goal> goals)
        {
            InitializeComponent();
            GoalsListBox.ItemsSource = goals;
        }

        private void GoalsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            OkButton.IsEnabled = GoalsListBox.SelectedItem != null;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedGoal = GoalsListBox.SelectedItem as Goal;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}