using System.Windows;
using financial_planner.ViewModels;

namespace financial_planner.View
{
    public partial class DistributionWindow : Window
    {
        public DistributionWindow()
        {
            InitializeComponent();
            DataContext = new DistributionViewModel();
        }
    }
}