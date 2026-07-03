using RestaurantApp.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.UI.Forms
{
    public partial class AllReservationsForm : Form
    {
        private readonly AppDbContext _context;
        private DataGridView dgvReservations = new();
        private Button btnRefresh = new();
        private Button btnClose = new();
        private Label lblCount = new();

        public AllReservationsForm()
        {
            _context = new AppDbContext();
            InitializeComponent();
            LoadReservations();
        }

        private void InitializeComponent()
        {
            this.Text = "📋 Все бронирования - Ресторан";
            this.Size = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            int y = 20;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "📋 Все бронирования",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Количество броней
            lblCount = new Label
            {
                Text = "Всего броней: 0",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 55),
                AutoSize = true
            };
            this.Controls.Add(lblCount);

            y = 80;

            // DataGridView
            dgvReservations = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(1050, 430),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Колонки
            dgvReservations.Columns.Add("Id", "№");
            dgvReservations.Columns.Add("Table", "Стол");
            dgvReservations.Columns.Add("Client", "Клиент");
            dgvReservations.Columns.Add("Date", "Дата");
            dgvReservations.Columns.Add("Time", "Время");
            dgvReservations.Columns.Add("Guests", "Гостей");
            dgvReservations.Columns.Add("Status", "Статус");

            dgvReservations.Columns["Id"].Width = 50;
            dgvReservations.Columns["Table"].Width = 80;
            dgvReservations.Columns["Client"].Width = 150;
            dgvReservations.Columns["Date"].Width = 120;
            dgvReservations.Columns["Time"].Width = 80;
            dgvReservations.Columns["Guests"].Width = 80;
            dgvReservations.Columns["Status"].Width = 120;

            this.Controls.Add(dgvReservations);

            y += 450;

            // Кнопка обновления
            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(20, y),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnRefresh.Click += BtnRefresh_Click!;
            this.Controls.Add(btnRefresh);

            // Кнопка закрытия
            btnClose = new Button
            {
                Text = "✖ Закрыть",
                Location = new Point(160, y),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadReservations();
        }

        private void LoadReservations()
        {
            try
            {
                dgvReservations.Rows.Clear();

                var reservations = _context.Reservations
                    .Include(r => r.Table)
                    .Include(r => r.User)
                    .OrderByDescending(r => r.ReservationDate)
                    .ThenBy(r => r.ReservationTime)
                    .ToList();

                if (reservations.Count == 0)
                {
                    dgvReservations.Rows.Add("", "Нет броней", "", "", "", "", "");
                    lblCount.Text = "Всего броней: 0";
                    return;
                }

                foreach (var r in reservations)
                {
                    string statusColor = r.Status == "Активна" ? "🟢" :
                                        r.Status == "Завершена" ? "🔵" :
                                        r.Status == "Отменена" ? "🔴" : "⚪";

                    dgvReservations.Rows.Add(
                        r.ReservationID,
                        $"#{r.Table?.TableNumber ?? 0}",
                        r.User?.FullName ?? "Неизвестно",
                        r.ReservationDate.ToString("dd.MM.yyyy"),
                        r.ReservationTime.ToString(@"hh\:mm"),
                        r.GuestCount,
                        $"{statusColor} {r.Status}"
                    );
                }

                lblCount.Text = $"Всего броней: {reservations.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки броней: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}