using System.Linq;
using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class TransactionArchiveWindow : Window
    {
        public TransactionArchiveWindow()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            var userId = AppData.CurrentUser.Id;

            var incomes = AppData.GetUserTransactionsByType(userId, TransactionType.Income)
                .OrderByDescending(t => t.Date)
                .ToList();

            var expenses = AppData.GetUserTransactionsByType(userId, TransactionType.Expense)
                .OrderByDescending(t => t.Date)
                .ToList();

            IncomeList.ItemsSource = incomes;
            ExpenseList.ItemsSource = expenses;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}