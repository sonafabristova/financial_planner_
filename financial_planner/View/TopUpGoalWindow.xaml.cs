using System.Windows;
using financial_planner.Models;
using financial_planner.ViewModels;

namespace financial_planner.View
{
    public partial class TopUpGoalWindow : Window
    {
        public TopUpGoalWindow(Goal goal)
        {
            InitializeComponent();
            var viewModel = new TopUpGoalViewModel(goal);
            DataContext = viewModel;
        }
    }
}