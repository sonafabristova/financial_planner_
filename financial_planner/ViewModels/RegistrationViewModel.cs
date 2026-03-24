using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _email;
        private string _fullName;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegistrationViewModel()
        {
            RegisterCommand = new RelayCommand(ExecuteRegister, CanExecuteRegister);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private bool CanExecuteRegister(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !string.IsNullOrWhiteSpace(FullName) &&
                   Password == ConfirmPassword;
        }

        private void ExecuteRegister(object parameter)
        {
            
            if (!string.IsNullOrEmpty(Email) && !Email.Contains("@"))
            {
                ErrorMessage = "Введите корректный email";
                return;
            }

            bool success = AppData.RegisterUser(Username, Password, Email, FullName);

            if (success)
            {
                MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.",
                              "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                (parameter as Window)?.Close();
            }
            else
            {
                ErrorMessage = "Пользователь с таким логином уже существует";
            }
        }

        private void ExecuteCancel(object parameter)
        {
            (parameter as Window)?.Close();
        }
    }
}