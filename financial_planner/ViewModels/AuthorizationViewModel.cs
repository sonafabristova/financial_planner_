using System.Windows;
using System.Windows.Input;
using financial_planner.Models;
using financial_planner.ViewModels.Base;

namespace financial_planner.ViewModels
{
    public class AuthorizationViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public AuthorizationViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister);
        }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object parameter)
        {
            var user = AppData.AuthenticateUser(Username, Password);

            if (user != null)
            {
                AppData.CurrentUser = user;

                MessageBox.Show($"Добро пожаловать, {user.FullName}!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                (parameter as Window)?.Close();
            }
            else
            {
                ErrorMessage = "Неверный логин или пароль";
            }
        }

        private void ExecuteRegister(object parameter)
        {
            var regWindow = new View.RegistrationWindow();
            regWindow.Owner = parameter as Window;
            regWindow.ShowDialog();
        }
    }
}