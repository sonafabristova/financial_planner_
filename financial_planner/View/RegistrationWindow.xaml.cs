using System.Windows;
using financial_planner.Models;
using financial_planner.ViewModels;

namespace financial_planner.View
{
    public partial class RegistrationWindow : Window
    {
        private DatabaseService _dbService;

        public RegistrationWindow()
        {
            InitializeComponent();
            DataContext = new RegistrationViewModel();
            _dbService = DatabaseService.Instance;
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BoxLogin.Text) &&
                !string.IsNullOrEmpty(BoxPass.Password) &&
                !string.IsNullOrEmpty(BoxFullName.Text))
            {
                var user = new User
                {
                    Username = BoxLogin.Text,
                    Password = BoxPass.Password,
                    FullName = BoxFullName.Text,
                    Email = BoxEmail.Text,
                    RegistrationDate = System.DateTime.Now
                };

                var account = new Account
                {
                    CurrentBalance = 0,
                    MonthlyIncome = 0,
                    MonthlyExpenses = 0,
                    LastUpdated = System.DateTime.Now
                };

                if (_dbService.RegisterUser(user, account))
                {
                    MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.",
                                  "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все обязательные поля!", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}