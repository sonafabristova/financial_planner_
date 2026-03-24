using System.Windows;
using financial_planner.Models;

namespace financial_planner.View 
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void ButtonCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BoxLogin.Text))
            {
                MessageBox.Show("Введите логин", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BoxPass.Password))
            {
                MessageBox.Show("Введите пароль", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BoxPass.Password != BoxConfirmPass.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BoxFullName.Text))
            {
                MessageBox.Show("Введите ФИО", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AppData.RegisterUser(BoxLogin.Text, BoxPass.Password, BoxEmail.Text, BoxFullName.Text))
            {
                MessageBox.Show("Регистрация прошла успешно!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}