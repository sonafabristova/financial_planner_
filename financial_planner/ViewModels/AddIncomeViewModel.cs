using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class AddIncomeViewModel : ViewModelBase
    {
        private readonly int _userId;
        private decimal _amount;
        private decimal _tax;
        private TransactionCategory _selectedCategory;
        private DateTime _selectedDate;
        private string _note;
        private string _errorMessage;
        private DatabaseService _dbService;

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (SetProperty(ref _amount, value))
                {
                    UpdateTax();
                }
            }
        }

        public decimal Tax
        {
            get => _tax;
            set => SetProperty(ref _tax, value);
        }

        public string TaxText => $"{Tax:N2} ₽";

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

        public List<TransactionCategory> IncomeCategories { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand HelpCommand { get; }
        public ICommand ArchiveCommand { get; }

        public AddIncomeViewModel()
        {
            _userId = AppState.CurrentUser?.Id ?? 0;
            _selectedDate = DateTime.Now;
            _dbService = DatabaseService.Instance;

            IncomeCategories = _dbService.GetIncomeCategories();
            SelectedCategory = IncomeCategories.FirstOrDefault();

            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);
            HelpCommand = new RelayCommand(ExecuteHelp);
            ArchiveCommand = new RelayCommand(ExecuteArchive);
        }

        private void UpdateTax()
        {
            Tax = Amount * 0.13m;
            OnPropertyChanged(nameof(TaxText));
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

                MessageBox.Show($"Доход в размере {Amount:N2} ₽ успешно добавлен", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                // ПРИНУДИТЕЛЬНОЕ ОБНОВЛЕНИЕ ГЛАВНОГО ОКНА
                var mainWindow = Application.Current.Windows.OfType<View.MainWindow>().FirstOrDefault();
                if (mainWindow?.DataContext is MainViewModel vm)
                {
                    vm.LoadData();
                }

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
            MessageBox.Show("Введите сумму дохода.\nНалог рассчитывается автоматически (13%).\nВыберите источник и дату получения дохода.",
                          "Помощь", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExecuteArchive(object parameter)
        {
            var archiveWindow = new View.TransactionArchiveWindow();
            archiveWindow.ShowDialog();
        }
    }
}