using RestaurantApp.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.UI.Forms
{
    public partial class UsersForm : Form
    {
        private readonly AppDbContext _context;
        private DataGridView dgvUsers = new();

        public UsersForm()
        {
            _context = new AppDbContext();
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.Text = "👥 Все пользователи - Ресторан";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var lblTitle = new Label
            {
                Text = "👥 Список всех пользователей",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            dgvUsers = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(850, 430),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvUsers.Columns.Add("Id", "ID");
            dgvUsers.Columns.Add("Login", "Логин");
            dgvUsers.Columns.Add("Name", "ФИО");
            dgvUsers.Columns.Add("Role", "Роль");
            dgvUsers.Columns.Add("Active", "Активен");

            this.Controls.Add(dgvUsers);
        }

        private void LoadUsers()
        {
            try
            {
                dgvUsers.Rows.Clear();

                var users = _context.Users
                    .Include(u => u.Role)
                    .OrderBy(u => u.RoleID)
                    .ToList();

                foreach (var user in users)
                {
                    dgvUsers.Rows.Add(
                        user.UserID,
                        user.Login,
                        user.FullName,
                        user.Role?.RoleName ?? "Без роли",
                        user.IsActive ? "✅" : "❌"
                    );
                }
            }
            catch
            {
                // Заглушка
            }
        }
    }
}