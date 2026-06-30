using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using RestaurantApp.Services;

namespace RestaurantApp.UI.Forms
{
    public partial class LoginForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AuthService _authService;

        public LoginForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _authService = (AuthService)serviceProvider.GetService(typeof(AuthService));
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string login = txtLogin.Text.Trim();
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(login))
                {
                    MessageBox.Show("Введите логин!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Введите пароль!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_authService == null)
                {
                    MessageBox.Show("Ошибка сервиса авторизации!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Вход (пароль НЕ ПРОВЕРЯЕТСЯ!)
                var user = await _authService.LoginAsync(login, password);

                if (user == null)
                {
                    MessageBox.Show($"Пользователь '{login}' не найден!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!user.IsActive)
                {
                    MessageBox.Show("Учетная запись заблокирована!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Успешный вход
                var mainForm = new MainForm(_serviceProvider, user);
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (_authService == null)
                {
                    MessageBox.Show("Ошибка сервиса авторизации!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var registerForm = new RegisterForm(_authService);
                registerForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}