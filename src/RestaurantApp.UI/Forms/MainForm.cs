using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Models;
using RestaurantApp.Data.Context;
using RestaurantApp.Services;
using RestaurantApp.UI.Forms;

namespace RestaurantApp.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly User _currentUser;
        private readonly OrderService _orderService;
        private readonly AppDbContext _context;

        // Элементы управления
        private Label lblWelcome = new();
        private Label lblRole = new();
        private Label lblUserInfo = new();
        private Panel pnlHeader = new();
        private Panel pnlContent = new();
        private Panel pnlSidebar = new();

        // Кнопки меню
        private Button btnOrders = new();
        private Button btnTables = new();
        private Button btnReservations = new();
        private Button btnMenu = new();
        private Button btnReports = new();
        private Button btnUsers = new();
        private Button btnAllReservations = new();
        private Button btnLogout = new();

        // Панель для отображения контента
        private Panel pnlWorkspace = new();

        // Статистические карточки
        private Label lblActiveOrders = new();
        private Label lblFreeTables = new();
        private Label lblTodayReservations = new();
        private Label lblTodayRevenue = new();

        public MainForm(IServiceProvider serviceProvider, User user)
        {
            _serviceProvider = serviceProvider;
            _currentUser = user;
            _orderService = (OrderService)serviceProvider.GetService(typeof(OrderService))!;
            _context = new AppDbContext();

            InitializeComponent();
            ShowWelcomeMessage();
            UpdateStatistics();
        }

        private void InitializeComponent()
        {
            this.Text = "Главное меню - Ресторан";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);
            this.BackColor = Color.FromArgb(245, 245, 245);

            // ============================================================
            // ВЕРХНЯЯ ПАНЕЛЬ (Header)
            // ============================================================
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(44, 62, 80),
                Padding = new Padding(20, 10, 20, 10)
            };

            // Заголовок
            var lblTitle = new Label
            {
                Text = "🍽️ СИСТЕМА УПРАВЛЕНИЯ РЕСТОРАНОМ",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Информация о пользователе
            lblUserInfo = new Label
            {
                Text = $"{_currentUser.FullName} ({_currentUser.Role?.RoleName ?? "Без роли"})",
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 50),
                AutoSize = true
            };

            // Кнопка выхода
            btnLogout = new Button
            {
                Text = "🚪 Выйти",
                Location = new Point(pnlHeader.Width - 120, 20),
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnLogout.Click += BtnLogout_Click!;

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblUserInfo);
            pnlHeader.Controls.Add(btnLogout);

            // ============================================================
            // БОКОВАЯ ПАНЕЛЬ (Sidebar)
            // ============================================================
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(10)
            };

            // ============================================================
            // КНОПКИ В БОКОВОЙ ПАНЕЛИ
            // ============================================================

            // Кнопка "Заказы"
            btnOrders = CreateSidebarButton("📋 Заказы", 0);
            btnOrders.Click += BtnOrders_Click!;

            // Кнопка "Схема зала"
            btnTables = CreateSidebarButton("🪑 Схема зала", 1);
            btnTables.Click += BtnTables_Click!;

            // Кнопка "Бронирования"
            btnReservations = CreateSidebarButton("📅 Бронирования", 2);
            btnReservations.Click += BtnReservations_Click!;

            // Кнопка "Меню" (только для администратора)
            if (_currentUser.RoleID == 5)
            {
                btnMenu = CreateSidebarButton("🍕 Управление меню", 3);
                btnMenu.Click += BtnMenu_Click!;
            }

            // Кнопка "Отчеты" (только для администратора)
            if (_currentUser.RoleID == 5)
            {
                btnReports = CreateSidebarButton("📊 Отчеты", 4);
                btnReports.Click += BtnReports_Click!;
            }

            // Кнопка "Пользователи" (только для администратора)
            if (_currentUser.RoleID == 5)
            {
                btnUsers = CreateSidebarButton("👥 Пользователи", 5);
                btnUsers.Click += BtnUsers_Click!;
            }

            // Кнопка "Все брони" (только для администратора)
            if (_currentUser.RoleID == 5)
            {
                btnAllReservations = CreateSidebarButton("📋 Все брони", 6);
                btnAllReservations.Click += BtnAllReservations_Click!;
            }

            // ============================================================
            // РАБОЧАЯ ОБЛАСТЬ
            // ============================================================
            pnlWorkspace = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // ============================================================
            // СБОРКА ФОРМЫ
            // ============================================================
            this.Controls.Add(pnlWorkspace);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlHeader);

            this.Resize += MainForm_Resize!;
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            btnLogout.Location = new Point(pnlHeader.Width - 120, 20);
        }

        // ============================================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // ============================================================

        private Button CreateSidebarButton(string text, int index)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Margin = new Padding(0, 5, 0, 5)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(52, 152, 219);

            pnlSidebar.Controls.Add(btn);
            return btn;
        }

        // ============================================================
        // ОБНОВЛЕНИЕ СТАТИСТИКИ
        // ============================================================

        private void UpdateStatistics()
        {
            try
            {
                // 1. Активные заказы (не отменены и не завершены)
                int activeOrders = _context.Orders
                    .Count(o => o.OrderStatus != "Отменен" && o.OrderStatus != "Готов");

                // 2. Свободные столы
                int freeTables = _context.Tables
                    .Count(t => t.Status == "Свободен");

                // 3. Брони на сегодня
                int todayReservations = _context.Reservations
                    .Count(r => r.ReservationDate.Date == DateTime.Today && r.Status == "Активна");

                // 4. Выручка сегодня
                decimal todayRevenue = _context.Payments
                    .Where(p => p.PaymentDate.Date == DateTime.Today)
                    .Sum(p => p.Amount);

                // Обновляем карточки
                UpdateStatCard(lblActiveOrders, "📋", "Активных заказов", activeOrders.ToString());
                UpdateStatCard(lblFreeTables, "🪑", "Свободных столов", freeTables.ToString());
                UpdateStatCard(lblTodayReservations, "📅", "Броней сегодня", todayReservations.ToString());
                UpdateStatCard(lblTodayRevenue, "💰", "Выручка сегодня", $"{todayRevenue:F2} ₽");
            }
            catch (Exception ex)
            {
                // Если БД не доступна, показываем заглушки
                UpdateStatCard(lblActiveOrders, "📋", "Активных заказов", "0");
                UpdateStatCard(lblFreeTables, "🪑", "Свободных столов", "0");
                UpdateStatCard(lblTodayReservations, "📅", "Броней сегодня", "0");
                UpdateStatCard(lblTodayRevenue, "💰", "Выручка сегодня", "0 ₽");
            }
        }

        private void UpdateStatCard(Label label, string icon, string title, string value)
        {
            if (label != null)
            {
                label.Text = $"{icon} {title}\n{value}";
                label.TextAlign = ContentAlignment.MiddleCenter;
            }
        }

        private void ShowWelcomeMessage()
        {
            pnlWorkspace.Controls.Clear();

            // ============================================================
            // ПРИВЕТСТВИЕ
            // ============================================================
            var lblWelcome = new Label
            {
                Text = $"👋 Добро пожаловать, {_currentUser.FullName}!",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblRole = new Label
            {
                Text = $"Ваша роль: {_currentUser.Role?.RoleName ?? "Не назначена"}",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblDate = new Label
            {
                Text = $"Сегодня: {DateTime.Now:dddd, dd MMMM yyyy HH:mm}",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(150, 150, 150),
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ============================================================
            // СТАТИСТИЧЕСКИЕ КАРТОЧКИ
            // ============================================================
            var pnlStats = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = Color.FromArgb(248, 249, 250),
                Margin = new Padding(0, 20, 0, 0)
            };

            var lblStats = new Label
            {
                Text = "📊 Статистика работы",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 10),
                AutoSize = true
            };
            pnlStats.Controls.Add(lblStats);

            // Создаём 4 карточки
            int cardY = 50;
            int cardWidth = 170;
            int cardHeight = 100;
            int spacing = 20;
            int startX = 20;

            // Карточка 1: Активные заказы
            lblActiveOrders = CreateStatCard(pnlStats, "📋", "Активных заказов", "0",
                Color.FromArgb(52, 152, 219), startX, cardY, cardWidth, cardHeight);

            // Карточка 2: Свободные столы
            lblFreeTables = CreateStatCard(pnlStats, "🪑", "Свободных столов", "0",
                Color.FromArgb(46, 204, 113), startX + cardWidth + spacing, cardY, cardWidth, cardHeight);

            // Карточка 3: Брони сегодня
            lblTodayReservations = CreateStatCard(pnlStats, "📅", "Броней сегодня", "0",
                Color.FromArgb(155, 89, 182), startX + (cardWidth + spacing) * 2, cardY, cardWidth, cardHeight);

            // Карточка 4: Выручка сегодня
            lblTodayRevenue = CreateStatCard(pnlStats, "💰", "Выручка сегодня", "0 ₽",
                Color.FromArgb(241, 196, 15), startX + (cardWidth + spacing) * 3, cardY, cardWidth, cardHeight);

            // ============================================================
            // СБОРКА
            // ============================================================
            pnlWorkspace.Controls.Add(pnlStats);
            pnlWorkspace.Controls.Add(lblDate);
            pnlWorkspace.Controls.Add(lblRole);
            pnlWorkspace.Controls.Add(lblWelcome);

            // Обновляем статистику
            UpdateStatistics();
        }

        private Label CreateStatCard(Panel parent, string icon, string title, string value, Color color, int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                Location = new Point(10, 5),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(10, 45),
                AutoSize = true
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(10, 70),
                AutoSize = true
            };

            panel.Controls.Add(lblIcon);
            panel.Controls.Add(lblValue);
            panel.Controls.Add(lblTitle);
            parent.Controls.Add(panel);

            // Возвращаем lblValue для обновления
            return lblValue;
        }

        // ============================================================
        // ОБРАБОТЧИКИ КНОПОК
        // ============================================================

        private void BtnOrders_Click(object? sender, EventArgs e)
        {
            try
            {
                var orderForm = new OrderForm(_orderService, _currentUser);
                orderForm.ShowDialog();
                UpdateStatistics(); // Обновляем статистику после закрытия
                ShowWelcomeMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTables_Click(object? sender, EventArgs e)
        {
            try
            {
                var tableMapForm = new TableMapForm();
                tableMapForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReservations_Click(object? sender, EventArgs e)
        {
            try
            {
                var reservationForm = new ReservationForm(_serviceProvider, _currentUser);
                reservationForm.ShowDialog();
                UpdateStatistics();
                ShowWelcomeMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMenu_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentUser.RoleID != 5)
                {
                    MessageBox.Show("Доступ запрещен! Только для администратора.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var menuForm = new MenuManagementForm();
                menuForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReports_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentUser.RoleID != 5)
                {
                    MessageBox.Show("Доступ запрещен! Только для администратора.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var reportsForm = new ReportsForm();
                reportsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUsers_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentUser.RoleID != 5)
                {
                    MessageBox.Show("Доступ запрещен! Только для администратора.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var usersForm = new UsersForm();
                usersForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAllReservations_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentUser.RoleID != 5)
                {
                    MessageBox.Show("Доступ запрещен! Только для администратора.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var allReservationsForm = new AllReservationsForm();
                allReservationsForm.ShowDialog();
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
                Application.Restart();
            }
        }
    }
}