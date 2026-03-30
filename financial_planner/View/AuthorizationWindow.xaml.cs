using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(BoxLogin.Text) && !string.IsNullOrWhiteSpace(BoxPass.Password))
            {
                var user = AppData.AuthenticateUser(BoxLogin.Text, BoxPass.Password);

                if (user != null)
                {
                    AppData.CurrentUser = user;

                    MainWindow mainWindow = new MainWindow();
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
            RegistrationWindow regWindow = new RegistrationWindow();
            regWindow.ShowDialog();

            if (regWindow.DialogResult == true && regWindow.RegUser != null)
            {
                BoxLogin.Text = regWindow.RegUser.Username;
                BoxPass.Password = regWindow.RegUser.Password;
            }
        }
    }
}