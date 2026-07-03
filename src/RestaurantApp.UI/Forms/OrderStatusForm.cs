using RestaurantApp.Core.Models;
using RestaurantApp.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.UI.Forms
{
    public partial class OrderStatusForm : Form
    {
        private readonly AppDbContext _context;
        private readonly User _currentUser;
        private System.Windows.Forms.Timer _timer;  // ← ЯВНО УКАЗЫВАЕМ ПРОСТРАНСТВО ИМЁН!
        private DataGridView dgvOrders = new();
        private Label lblStatusInfo = new();
        private Button btnRefresh = new();
        private Label lblTimer = new();

        public OrderStatusForm(User currentUser)
        {
            _currentUser = currentUser;
            _context = new AppDbContext();
            InitializeComponent();
            LoadOrders();
            StartTimer();
        }

        private void InitializeComponent()
        {
            this.Text = "📋 Статус моих заказов - Ресторан";
            this.Size = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            int y = 20;

            var lblTitle = new Label
            {
                Text = "📋 Статус моих заказов",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Информация о статусе
            lblStatusInfo = new Label
            {
                Text = "🟢 Ваши заказы:",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, 70),
                AutoSize = true
            };
            this.Controls.Add(lblStatusInfo);

            // Таймер
            lblTimer = new Label
            {
                Text = "🔄 Обновлено: " + DateTime.Now.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(800, 72),
                AutoSize = true
            };
            this.Controls.Add(lblTimer);

            y = 100;

            // Таблица заказов
            dgvOrders = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(1050, 380),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 249, 250) }
            };

            // Настройка колонок
            dgvOrders.Columns.Add("Id", "№ заказа");
            dgvOrders.Columns.Add("Table", "Стол");
            dgvOrders.Columns.Add("Date", "Дата");
            dgvOrders.Columns.Add("Status", "Статус");
            dgvOrders.Columns.Add("Time", "Время ожидания");
            dgvOrders.Columns.Add("Dishes", "Блюда");

            dgvOrders.Columns["Id"].Width = 80;
            dgvOrders.Columns["Table"].Width = 60;
            dgvOrders.Columns["Date"].Width = 120;
            dgvOrders.Columns["Status"].Width = 150;
            dgvOrders.Columns["Time"].Width = 120;
            dgvOrders.Columns["Dishes"].Width = 400;

            // Цветовая подсветка статусов
            dgvOrders.CellFormatting += DgvOrders_CellFormatting!;

            this.Controls.Add(dgvOrders);

            y += 400;

            // Кнопка обновления
            btnRefresh = new Button
            {
                Text = "🔄 Обновить статус",
                Location = new Point(20, y),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnRefresh.Click += BtnRefresh_Click!;
            this.Controls.Add(btnRefresh);

            // Легенда статусов
            var pnlLegend = new Panel
            {
                Location = new Point(200, y),
                Size = new Size(700, 35),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            AddLegend(pnlLegend, Color.Orange, "🟠 Составление", 10);
            AddLegend(pnlLegend, Color.Blue, "🔵 Оформлен", 130);
            AddLegend(pnlLegend, Color.FromArgb(52, 152, 219), "🔵 Принят", 250);
            AddLegend(pnlLegend, Color.FromArgb(241, 196, 15), "🟡 Готовится", 370);
            AddLegend(pnlLegend, Color.Green, "🟢 Готов", 490);
            AddLegend(pnlLegend, Color.Red, "🔴 Отменен", 610);

            this.Controls.Add(pnlLegend);
        }

        private void AddLegend(Panel parent, Color color, string text, int x)
        {
            var panel = new Panel
            {
                Location = new Point(x, 5),
                Size = new Size(100, 25)
            };

            var box = new Panel
            {
                Location = new Point(0, 4),
                Size = new Size(15, 15),
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label
            {
                Text = text,
                Location = new Point(20, 2),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            panel.Controls.Add(box);
            panel.Controls.Add(label);
            parent.Controls.Add(panel);
        }

        private void DgvOrders_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 3) return;

            var status = e.Value?.ToString() ?? "";
            var row = dgvOrders.Rows[e.RowIndex];

            switch (status)
            {
                case "Составление":
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                case "Оформлен":
                    row.DefaultCellStyle.BackColor = Color.Blue;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                case "Принят":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                case "Готовится":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(241, 196, 15);
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
                case "Готов":
                    row.DefaultCellStyle.BackColor = Color.Green;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                case "Выдан Клиенту":
                    row.DefaultCellStyle.BackColor = Color.FromArgb(46, 204, 113);
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                case "Отменен":
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                default:
                    row.DefaultCellStyle.BackColor = Color.Gray;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
            }
        }

        private void LoadOrders()
        {
            try
            {
                dgvOrders.Rows.Clear();

                // ============================================================
                // ИСПРАВЛЕНО: Убираем навигационное свойство Reservation
                // Используем прямые запросы через ReservationID
                // ============================================================
                var orders = _context.Orders
                    .Where(o => o.ReservationID != null)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                // Фильтруем по клиенту через Reservation
                var userReservations = _context.Reservations
                    .Where(r => r.UserID == _currentUser.UserID)
                    .Select(r => r.ReservationID)
                    .ToList();

                var userOrders = orders
                    .Where(o => o.ReservationID.HasValue && userReservations.Contains(o.ReservationID.Value))
                    .ToList();

                if (userOrders.Count == 0)
                {
                    dgvOrders.Rows.Add("", "", "", "У вас нет заказов", "", "");
                    lblStatusInfo.Text = "📭 У вас пока нет заказов";
                    return;
                }

                int activeCount = 0;
                int readyCount = 0;

                foreach (var order in userOrders)
                {
                    var status = order.OrderStatus;

                    // Получаем номер стола через Reservation
                    var reservation = _context.Reservations
                        .Include(r => r.Table)
                        .FirstOrDefault(r => r.ReservationID == order.ReservationID);

                    var tableNum = reservation?.Table?.TableNumber ?? 0;
                    var orderDate = order.OrderDate;
                    var dishNames = string.Join(", ", order.OrderItems
                        .Where(oi => oi.MenuItem != null)
                        .Select(oi => $"{oi.MenuItem?.Name} x{oi.Quantity}"));

                    var waitTime = GetWaitTime(order);

                    dgvOrders.Rows.Add(
                        order.OrderID,
                        $"#{tableNum}",
                        orderDate.ToString("dd.MM.yyyy HH:mm"),
                        GetStatusWithIcon(status),
                        waitTime,
                        dishNames
                    );

                    if (status == "Готовится" || status == "Принят")
                        activeCount++;
                    if (status == "Готов")
                        readyCount++;
                }

                lblStatusInfo.Text = $"🟢 Ваши заказы: {userOrders.Count} | Готовится: {activeCount} | Готово: {readyCount}";
                lblTimer.Text = "🔄 Обновлено: " + DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetStatusWithIcon(string status)
        {
            return status switch
            {
                "Составление" => "🟠 " + status,
                "Оформлен" => "🔵 " + status,
                "Принят" => "🔵 " + status,
                "Готовится" => "🟡 " + status,
                "Готов" => "🟢 " + status,
                "Выдан Клиенту" => "✅ " + status,
                "Отменен" => "❌ " + status,
                _ => "⚪ " + status
            };
        }

        private string GetWaitTime(Order order)
        {
            if (order.OrderStatus == "Готов" || order.OrderStatus == "Выдан Клиенту")
                return "✅ Готов!";

            if (order.OrderStatus == "Отменен")
                return "❌ Отменен";

            var elapsed = DateTime.Now - order.OrderDate;
            var minutes = (int)elapsed.TotalMinutes;

            if (minutes < 10)
                return "⏳ ~10-15 мин";
            else if (minutes < 20)
                return "⏳ ~5-10 мин";
            else if (minutes < 30)
                return "⏳ ~1-5 мин";
            else if (minutes < 45)
                return "⏳ ~5-10 мин";
            else
                return "⏳ Скоро будет готов";
        }

        private void StartTimer()
        {
            _timer = new System.Windows.Forms.Timer();  // ← ЯВНО УКАЗЫВАЕМ ПРОСТРАНСТВО ИМЁН!
            _timer.Interval = 30000; // 30 секунд
            _timer.Tick += (s, e) => LoadOrders();
            _timer.Start();
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadOrders();
            MessageBox.Show("✅ Статус заказов обновлен!", "Успешно",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timer?.Stop();
            _timer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}