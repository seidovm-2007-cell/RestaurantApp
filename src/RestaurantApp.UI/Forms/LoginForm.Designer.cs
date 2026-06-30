using System.Windows.Forms;  // ← ДОБАВИТЬ!
using System.Drawing;        // ← ДОБАВИТЬ!
namespace RestaurantApp.UI.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        // Объявление элементов
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtLogin = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnRegister = new Button();
            this.SuspendLayout();

            // Заголовок
            Label lblTitle = new Label();
            lblTitle.Text = "🍽️ Вход в систему";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(50, 20);
            lblTitle.Size = new Size(300, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Лейбл логин
            Label lblLogin = new Label();
            lblLogin.Text = "Логин:";
            lblLogin.Font = new Font("Segoe UI", 11);
            lblLogin.Location = new Point(50, 80);
            lblLogin.Size = new Size(100, 25);

            // txtLogin
            this.txtLogin.Location = new Point(50, 110);
            this.txtLogin.Size = new Size(280, 25);
            this.txtLogin.Font = new Font("Segoe UI", 11);
            this.txtLogin.BackColor = Color.FromArgb(248, 249, 250);

            // Лейбл пароль
            Label lblPassword = new Label();
            lblPassword.Text = "Пароль:";
            lblPassword.Font = new Font("Segoe UI", 11);
            lblPassword.Location = new Point(50, 150);
            lblPassword.Size = new Size(100, 25);

            // txtPassword
            this.txtPassword.Location = new Point(50, 180);
            this.txtPassword.Size = new Size(280, 25);
            this.txtPassword.Font = new Font("Segoe UI", 11);
            this.txtPassword.BackColor = Color.FromArgb(248, 249, 250);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.UseSystemPasswordChar = true;

            // btnLogin
            this.btnLogin.Text = "Войти";
            this.btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            this.btnLogin.Location = new Point(50, 230);
            this.btnLogin.Size = new Size(130, 40);
            this.btnLogin.BackColor = Color.FromArgb(46, 204, 113);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.Click += new EventHandler(this.BtnLogin_Click);

            // btnRegister
            this.btnRegister.Text = "Регистрация";
            this.btnRegister.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            this.btnRegister.Location = new Point(200, 230);
            this.btnRegister.Size = new Size(130, 40);
            this.btnRegister.BackColor = Color.FromArgb(52, 152, 219);
            this.btnRegister.ForeColor = Color.White;
            this.btnRegister.FlatStyle = FlatStyle.Flat;
            this.btnRegister.Click += new EventHandler(this.BtnRegister_Click);

            // Добавление на форму
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblLogin);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnRegister);

            // Настройка формы
            this.Text = "Вход в систему - Ресторан";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(this.LoginForm_KeyDown);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }
    }
}