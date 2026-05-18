using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class AddExpenseViewModel : ViewModelBase
    {
        private readonly int _userId;
        private decimal _amount;
        private TransactionCategory _selectedCategory;
        private DateTime _selectedDate;
        private string _note;
        private string _errorMessage;
        private DatabaseService _dbService;

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public TransactionCategory SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public List<TransactionCategory> ExpenseCategories { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand HelpCommand { get; }
        public ICommand ArchiveCommand { get; }

        public AddExpenseViewModel()
        {
            _userId = AppState.CurrentUser?.Id ?? 0;
            _selectedDate = DateTime.Now;
            _dbService = DatabaseService.Instance;

            ExpenseCategories = _dbService.GetExpenseCategories();
            SelectedCategory = ExpenseCategories.FirstOrDefault();

            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
            HelpCommand = new RelayCommand(ExecuteHelp);
            ArchiveCommand = new RelayCommand(ExecuteArchive);
        }

        private bool CanExecuteSave(object parameter)
        {
            return Amount > 0;
        }

        private void ExecuteSave(object parameter)
        {
            try
            {
                var transaction = new Transaction
                {
                    UserId = _userId,
                    CategoryId = SelectedCategory.Id,
                    Amount = Amount,
                    Date = SelectedDate,
                    Note = Note ?? ""
                };

                _dbService.AddTransaction(transaction);

                MessageBox.Show($"Расход в размере {Amount:N2} ₽ успешно добавлен", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                (parameter as Window)?.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        private void ExecuteCancel(object parameter)
        {
            (parameter as Window)?.Close();
        }

        private void ExecuteHelp(object parameter)
        {
            MessageBox.Show("Введите сумму расхода.\nВыберите категорию и дату.",
                          "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteArchive(object parameter)
        {
            var archiveWindow = new View.TransactionArchiveWindow();
            archiveWindow.ShowDialog();
        }
    }
}