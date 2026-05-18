using System.Windows;
using financial_planner.Models;
using financial_planner.ViewModels;

namespace financial_planner.View
{
    public partial class AuthorizationWindow : Window
    {
        private DatabaseService _dbService;

        public AuthorizationWindow()
        {
            InitializeComponent();
            DataContext = new AuthorizationViewModel();
            _dbService = DatabaseService.Instance;
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(BoxLogin.Text) && !string.IsNullOrWhiteSpace(BoxPass.Password))
            {
                var user = _dbService.AuthenticateUser(BoxLogin.Text, BoxPass.Password);

                if (user != null)
                {
                    AppState.CurrentUser = user;

                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegistrationWindow();
            regWindow.Owner = this;
            regWindow.ShowDialog();
        }
    }
}