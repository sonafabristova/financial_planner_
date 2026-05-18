using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class TransactionArchiveViewModel : ViewModelBase
    {
        private readonly int _userId;
        private ObservableCollection<Transaction> _incomeTransactions;
        private ObservableCollection<Transaction> _expenseTransactions;
        private decimal _totalIncome;
        private decimal _totalExpense;
        private DatabaseService _dbService;

        public ObservableCollection<Transaction> IncomeTransactions
        {
            get => _incomeTransactions;
            set => SetProperty(ref _incomeTransactions, value);
        }

        public ObservableCollection<Transaction> ExpenseTransactions
        {
            get => _expenseTransactions;
            set => SetProperty(ref _expenseTransactions, value);
        }

        public decimal TotalIncome
        {
            get => _totalIncome;
            set => SetProperty(ref _totalIncome, value);
        }

        public string TotalIncomeText => $"Всего доходов: {TotalIncome:N0} ₽";

        public decimal TotalExpense
        {
            get => _totalExpense;
            set => SetProperty(ref _totalExpense, value);
        }

        public string TotalExpenseText => $"Всего расходов: {TotalExpense:N0} ₽";

        public ICommand CloseCommand { get; }

        public TransactionArchiveViewModel()
        {
            _userId = AppState.CurrentUser?.Id ?? 0;
            _dbService = DatabaseService.Instance;
            CloseCommand = new RelayCommand(ExecuteClose);

            LoadTransactions();
        }

        private void LoadTransactions()
        {
            var allTransactions = _dbService.GetUserTransactions(_userId);

            IncomeTransactions = new ObservableCollection<Transaction>(
                allTransactions.Where(t => t.Category.TypeId == 1));

            ExpenseTransactions = new ObservableCollection<Transaction>(
                allTransactions.Where(t => t.Category.TypeId == 2));

            TotalIncome = IncomeTransactions.Sum(t => t.Amount);
            TotalExpense = ExpenseTransactions.Sum(t => t.Amount);
        }

        private void ExecuteClose(object parameter)
        {
            (parameter as System.Windows.Window)?.Close();
        }
    }
}