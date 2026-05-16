using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels;

namespace financial_planner.View
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = DataContext as MainViewModel;
        }

        public void UpdateStatistics()
        {
            _viewModel?.LoadData();
        }

        public void LoadGoals()
        {
            _viewModel?.LoadData();
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
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            (_viewModel as MainViewModel)?.Cleanup();
        }
    }// бэкап сделать бд и закинуть в проект, этот выбор повлияет на историю в будущем
}