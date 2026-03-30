using System.Windows;
using financial_planner.Models;

namespace financial_planner.View
{
    public partial class RegistrationWindow : Window
    {
        public User? RegUser { get; set; } = null;

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BoxLogin.Text) &&
                !string.IsNullOrEmpty(BoxPass.Password) &&
                !string.IsNullOrEmpty(BoxFullName.Text))
            {
                RegUser = new User
                {
                    Id = AppData.Users.Count > 0 ? AppData.Users[AppData.Users.Count - 1].Id + 1 : 1,
                    Username = BoxLogin.Text,
                    Password = BoxPass.Password,
                    FullName = BoxFullName.Text,
                    Email = BoxEmail.Text,
                    RegistrationDate = System.DateTime.Now
                };

                if (AppData.RegisterUser(RegUser.Username, RegUser.Password, RegUser.Email, RegUser.FullName))
                {
                    DialogResult = true;
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
            DialogResult = false;
            this.Close();
        }
    }
}