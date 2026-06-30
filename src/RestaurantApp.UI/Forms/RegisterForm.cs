using RestaurantApp.Services;

namespace RestaurantApp.UI.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly AuthService _authService;
        private TextBox txtLogin = new TextBox();
        private TextBox txtPassword = new TextBox();
        private TextBox txtLastName = new TextBox();
        private TextBox txtFirstName = new TextBox();
        private TextBox txtMiddleName = new TextBox();
        private Button btnRegister = new Button();
        private Button btnCancel = new Button();
        private Label lblStatus = new Label();

        public RegisterForm(AuthService authService)
        {
            _authService = authService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Регистрация - Ресторан";
            this.Size = new Size(400, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            int y = 20;

            Label lblTitle = new Label();
            lblTitle.Text = "📝 Регистрация";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(50, y);
            lblTitle.Size = new Size(300, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            y += 50;

            // Логин
            AddField("Логин:", ref txtLogin, ref y);

            // Пароль
            AddField("Пароль:", ref txtPassword, ref y, true);

            // Фамилия
            AddField("Фамилия:", ref txtLastName, ref y);

            // Имя
            AddField("Имя:", ref txtFirstName, ref y);

            // Отчество
            AddField("Отчество:", ref txtMiddleName, ref y);

            // Статус
            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.Font = new Font("Segoe UI", 10);
            lblStatus.Location = new Point(50, y);
            lblStatus.Size = new Size(280, 25);
            lblStatus.ForeColor = Color.Red;
            this.Controls.Add(lblStatus);
            y += 35;

            // Кнопка регистрации
            btnRegister = new Button();
            btnRegister.Text = "Зарегистрироваться";
            btnRegister.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnRegister.Location = new Point(50, y);
            btnRegister.Size = new Size(180, 40);
            btnRegister.BackColor = Color.FromArgb(46, 204, 113);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Click += BtnRegister_Click!;
            this.Controls.Add(btnRegister);

            // Кнопка отмены
            btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnCancel.Location = new Point(240, y);
            btnCancel.Size = new Size(100, 40);
            btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            this.Controls.Add(lblTitle);
        }

        private void AddField(string labelText, ref TextBox textBox, ref int y, bool isPassword = false)
        {
            Label label = new Label();
            label.Text = labelText;
            label.Font = new Font("Segoe UI", 11);
            label.Location = new Point(50, y);
            label.Size = new Size(100, 25);
            this.Controls.Add(label);

            textBox = new TextBox();
            textBox.Location = new Point(50, y + 30);
            textBox.Size = new Size(280, 25);
            textBox.Font = new Font("Segoe UI", 11);
            textBox.BackColor = Color.FromArgb(248, 249, 250);

            if (isPassword)
            {
                textBox.PasswordChar = '*';
                textBox.UseSystemPasswordChar = true;
            }

            this.Controls.Add(textBox);
            y += 65;
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string login = txtLogin.Text.Trim();
                string password = txtPassword.Text;
                string lastName = txtLastName.Text.Trim();
                string firstName = txtFirstName.Text.Trim();
                string middleName = txtMiddleName.Text.Trim();

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName))
                {
                    lblStatus.Text = "⚠️ Заполните все обязательные поля!";
                    lblStatus.ForeColor = Color.Orange;
                    return;
                }

                if (password.Length < 6)
                {
                    lblStatus.Text = "⚠️ Пароль должен быть не менее 6 символов!";
                    lblStatus.ForeColor = Color.Orange;
                    return;
                }

                var user = await _authService.RegisterAsync(login, password, lastName, firstName, middleName);

                MessageBox.Show($"✅ Регистрация успешна!\n\nДобро пожаловать, {user.FullName}!",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ " + ex.Message;
                lblStatus.ForeColor = Color.Red;
            }
        }
    }
}