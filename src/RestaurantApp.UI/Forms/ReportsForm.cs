using RestaurantApp.Data.Context;
using RestaurantApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.UI.Forms
{
    public partial class ReportsForm : Form
    {
        private readonly AppDbContext _context;
        private readonly User _currentUser;
        private ComboBox cmbReportType = new();
        private ComboBox cmbPeriod = new();
        private Button btnGenerate = new();
        private DataGridView dgvReport = new();
        private Label lblInfo = new();
        private Label lblTotal = new();

        public ReportsForm(User currentUser)
        {
            _currentUser = currentUser;
            _context = new AppDbContext();
            InitializeComponent();
            SetupAccessRights();
            GenerateReport();
        }

        private void InitializeComponent()
        {
            this.Text = "📊 Отчеты и статистика - Ресторан";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            int y = 20;

            var lblTitle = new Label
            {
                Text = "📊 Отчеты и статистика",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            y += 50;

            var pnlFilters = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(1050, 50),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblType = new Label
            {
                Text = "Тип отчета:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlFilters.Controls.Add(lblType);

            cmbReportType = new ComboBox
            {
                Location = new Point(140, 12),
                Size = new Size(220, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbReportType.SelectedIndexChanged += CmbReportType_SelectedIndexChanged!;
            pnlFilters.Controls.Add(cmbReportType);

            var lblPeriod = new Label
            {
                Text = "Период:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(400, 15),
                AutoSize = true
            };
            pnlFilters.Controls.Add(lblPeriod);

            cmbPeriod = new ComboBox
            {
                Location = new Point(490, 12),
                Size = new Size(180, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbPeriod.Items.Add("За сегодня");
            cmbPeriod.Items.Add("За неделю");
            cmbPeriod.Items.Add("За месяц");
            cmbPeriod.Items.Add("За 3 месяца");
            cmbPeriod.Items.Add("За год");
            cmbPeriod.SelectedIndex = 2;
            pnlFilters.Controls.Add(cmbPeriod);

            btnGenerate = new Button
            {
                Text = "📊 Сформировать",
                Location = new Point(720, 6),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnGenerate.Click += BtnGenerate_Click!;
            pnlFilters.Controls.Add(btnGenerate);

            this.Controls.Add(pnlFilters);
            y += 70;

            lblInfo = new Label
            {
                Text = "📌 Выберите тип отчета и нажмите 'Сформировать'",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblInfo);
            y += 30;

            dgvReport = new DataGridView
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
            this.Controls.Add(dgvReport);

            y += 450;

            lblTotal = new Label
            {
                Text = "📊 Итого: 0.00 ₽",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblTotal);
        }

        private void SetupAccessRights()
        {
            if (_currentUser.RoleID == 8)
            {
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add("⭐ Популярность блюд");
                cmbReportType.Items.Add("📅 Бронирования по дням");
                cmbReportType.SelectedIndex = 0;
            }
            else if (_currentUser.RoleID == 6)
            {
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add("⭐ Популярность блюд");
                cmbReportType.Items.Add("🪑 Занятость столов");
                cmbReportType.Items.Add("📅 Бронирования по дням");
                cmbReportType.SelectedIndex = 0;
            }
            else
            {
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add("📈 Продажи по категориям");
                cmbReportType.Items.Add("👨‍🍳 Работа официантов");
                cmbReportType.Items.Add("⭐ Популярность блюд");
                cmbReportType.Items.Add("🪑 Занятость столов");
                cmbReportType.Items.Add("📅 Бронирования по дням");
                cmbReportType.SelectedIndex = 0;
            }
        }

        private void CmbReportType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            lblInfo.Text = "📌 Нажмите 'Сформировать' для обновления отчета";
        }

        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            try
            {
                string reportType = cmbReportType.SelectedItem?.ToString() ?? "";
                string period = cmbPeriod.SelectedItem?.ToString() ?? "За месяц";

                dgvReport.Columns.Clear();
                dgvReport.Rows.Clear();
                lblTotal.Text = "📊 Итого: 0.00 ₽";

                DateTime startDate = GetStartDate(period);
                DateTime endDate = DateTime.Now;

                lblInfo.Text = $"📌 Отчет: {reportType} | Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";

                switch (reportType)
                {
                    case "📈 Продажи по категориям":
                        GenerateSalesReport(startDate, endDate);
                        break;
                    case "👨‍🍳 Работа официантов":
                        GenerateWaiterReport(startDate, endDate);
                        break;
                    case "⭐ Популярность блюд":
                        GeneratePopularityReport(startDate, endDate);
                        break;
                    case "🪑 Занятость столов":
                        GenerateTableOccupancyReport();
                        break;
                    case "📅 Бронирования по дням":
                        GenerateReservationsReport(startDate, endDate);
                        break;
                    default:
                        dgvReport.Rows.Add("Выберите тип отчета");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DateTime GetStartDate(string period)
        {
            return period switch
            {
                "За сегодня" => DateTime.Today,
                "За неделю" => DateTime.Today.AddDays(-7),
                "За месяц" => DateTime.Today.AddMonths(-1),
                "За 3 месяца" => DateTime.Today.AddMonths(-3),
                "За год" => DateTime.Today.AddYears(-1),
                _ => DateTime.Today.AddMonths(-1)
            };
        }

        // ============================================================
        // ИСПРАВЛЕННЫЙ ОТЧЕТ — УЧИТЫВАЕТ ВСЕ ЗАКАЗЫ (кроме отменённых)
        // ============================================================
        private void GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            dgvReport.Columns.Add("Category", "Категория");
            dgvReport.Columns.Add("Dish", "Блюдо");
            dgvReport.Columns.Add("Quantity", "Кол-во продаж");
            dgvReport.Columns.Add("Revenue", "Выручка");
            dgvReport.Columns["Revenue"].DefaultCellStyle.Format = "F2";

            try
            {
                // ============================================================
                // ИСПРАВЛЕНИЕ: Убираем "Составление" из исключений!
                // Теперь учитываются все заказы, кроме отменённых
                // ============================================================
                var sales = _context.OrderItems
                    .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                    .Where(oi => oi.Order.OrderStatus != "Отменен")  // ← ТОЛЬКО ОТМЕНЁННЫЕ ИСКЛЮЧЕНЫ!
                    .Join(_context.MenuItems,
                        oi => oi.MenuItemID,
                        mi => mi.MenuItemID,
                        (oi, mi) => new { oi, mi })
                    .GroupBy(x => new { x.mi.Category, x.mi.Name })
                    .Select(g => new
                    {
                        Category = g.Key.Category,
                        Dish = g.Key.Name,
                        Quantity = g.Sum(x => x.oi.Quantity),
                        Revenue = g.Sum(x => x.oi.Quantity * x.oi.UnitPrice * (1 - x.oi.Discount / 100))
                    })
                    .OrderBy(x => x.Category)
                    .ThenBy(x => x.Dish)
                    .ToList();

                if (sales == null || sales.Count == 0)
                {
                    dgvReport.Rows.Add("Нет данных за выбранный период", "", "", "");
                    return;
                }

                decimal totalRevenue = 0;
                int totalQuantity = 0;

                foreach (var item in sales)
                {
                    dgvReport.Rows.Add(item.Category, item.Dish, item.Quantity, item.Revenue);
                    totalQuantity += item.Quantity;
                    totalRevenue += item.Revenue;
                }

                dgvReport.Rows.Add("━━━━━━━━━━━━━", "━━━━━━━━━━━━━━━━", "━━━━━━━━━━━━━━━", "━━━━━━━━━━━━━━━");
                dgvReport.Rows.Add("ИТОГО:", "", totalQuantity, totalRevenue);

                lblTotal.Text = $"📊 Общая выручка: {totalRevenue:F2} ₽ | Всего продаж: {totalQuantity}";
                lblTotal.ForeColor = Color.FromArgb(231, 76, 60);
            }
            catch (Exception ex)
            {
                dgvReport.Rows.Add($"Ошибка: {ex.Message}", "", "", "");
            }
        }

        private void GenerateWaiterReport(DateTime startDate, DateTime endDate)
        {
            dgvReport.Columns.Add("Waiter", "Официант");
            dgvReport.Columns.Add("Orders", "Заказов");
            dgvReport.Columns.Add("Payments", "Оплат");
            dgvReport.Columns.Add("Revenue", "Выручка");
            dgvReport.Columns["Revenue"].DefaultCellStyle.Format = "F2";

            try
            {
                var waiters = _context.Users
                    .Where(u => u.RoleID == 2)
                    .ToList();

                if (waiters == null || waiters.Count == 0)
                {
                    dgvReport.Rows.Add("Нет официантов", "", "", "");
                    return;
                }

                decimal totalRevenue = 0;
                int totalOrders = 0;
                int totalPayments = 0;

                foreach (var waiter in waiters)
                {
                    var orders = _context.Orders
                        .Where(o => o.Shift != null && o.Shift.UserID == waiter.UserID)
                        .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                        .Where(o => o.OrderStatus != "Отменен")
                        .ToList();

                    var orderIds = orders.Select(o => o.OrderID).ToList();

                    var payments = _context.Payments
                        .Where(p => orderIds.Contains(p.OrderID))
                        .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                        .ToList();

                    decimal waiterRevenue = payments.Sum(p => p.Amount);
                    int waiterOrders = orders.Count;
                    int waiterPayments = payments.Count;

                    if (waiterOrders > 0 || waiterPayments > 0 || waiterRevenue > 0)
                    {
                        dgvReport.Rows.Add(
                            waiter.FullName,
                            waiterOrders,
                            waiterPayments,
                            waiterRevenue
                        );
                        totalOrders += waiterOrders;
                        totalPayments += waiterPayments;
                        totalRevenue += waiterRevenue;
                    }
                }

                if (dgvReport.Rows.Count == 0)
                {
                    dgvReport.Rows.Add("Нет данных за выбранный период", "", "", "");
                    return;
                }

                dgvReport.Rows.Add("━━━━━━━━━━━━━", "━━━━━━━━━━━", "━━━━━━━━━━━", "━━━━━━━━━━━━━━━");
                dgvReport.Rows.Add("ИТОГО:", totalOrders, totalPayments, totalRevenue);

                lblTotal.Text = $"📊 Общая выручка официантов: {totalRevenue:F2} ₽ | Всего заказов: {totalOrders}";
                lblTotal.ForeColor = Color.FromArgb(231, 76, 60);
            }
            catch (Exception ex)
            {
                dgvReport.Rows.Add($"Ошибка: {ex.Message}", "", "", "");
            }
        }

        private void GeneratePopularityReport(DateTime startDate, DateTime endDate)
        {
            dgvReport.Columns.Add("Dish", "Блюдо");
            dgvReport.Columns.Add("Category", "Категория");
            dgvReport.Columns.Add("Orders", "Кол-во заказов");
            dgvReport.Columns.Add("Rating", "Рейтинг");

            try
            {
                var popularity = _context.OrderItems
                    .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                    .Where(oi => oi.Order.OrderStatus != "Отменен")
                    .Join(_context.MenuItems,
                        oi => oi.MenuItemID,
                        mi => mi.MenuItemID,
                        (oi, mi) => new { oi, mi })
                    .GroupBy(x => new { x.mi.Name, x.mi.Category })
                    .Select(g => new
                    {
                        Dish = g.Key.Name,
                        Category = g.Key.Category,
                        Quantity = g.Sum(x => x.oi.Quantity)
                    })
                    .OrderByDescending(x => x.Quantity)
                    .ToList();

                if (popularity == null || popularity.Count == 0)
                {
                    dgvReport.Rows.Add("Нет данных", "", "", "");
                    return;
                }

                int maxQty = popularity.FirstOrDefault()?.Quantity ?? 1;

                foreach (var item in popularity)
                {
                    int stars = (int)Math.Ceiling((double)item.Quantity / maxQty * 5);
                    string rating = new string('⭐', Math.Min(stars, 5));
                    if (string.IsNullOrEmpty(rating)) rating = "⭐";

                    dgvReport.Rows.Add(item.Dish, item.Category, item.Quantity, rating);
                }

                int totalQuantity = popularity.Sum(x => x.Quantity);
                lblTotal.Text = $"📊 Всего заказов: {totalQuantity} | Самое популярное: {popularity.FirstOrDefault()?.Dish}";
                lblTotal.ForeColor = Color.FromArgb(52, 152, 219);
            }
            catch (Exception ex)
            {
                dgvReport.Rows.Add($"Ошибка: {ex.Message}", "", "", "");
            }
        }

        private void GenerateTableOccupancyReport()
        {
            dgvReport.Columns.Add("Table", "Стол");
            dgvReport.Columns.Add("Capacity", "Вместимость");
            dgvReport.Columns.Add("Status", "Статус");
            dgvReport.Columns.Add("TodayReservations", "Броней сегодня");
            dgvReport.Columns.Add("TotalReservations", "Всего броней");

            try
            {
                var tables = _context.Tables
                    .Select(t => new
                    {
                        t.TableNumber,
                        t.Capacity,
                        t.Status,
                        TodayReservations = _context.Reservations
                            .Count(r => r.TableID == t.TableID && r.ReservationDate == DateTime.Today && r.Status != "Отменена"),
                        TotalReservations = _context.Reservations
                            .Count(r => r.TableID == t.TableID && r.Status != "Отменена")
                    })
                    .OrderBy(t => t.TableNumber)
                    .ToList();

                if (tables == null || tables.Count == 0)
                {
                    dgvReport.Rows.Add("Нет данных", "", "", "", "");
                    return;
                }

                int totalToday = 0;
                int totalAll = 0;

                foreach (var t in tables)
                {
                    string statusIcon = t.Status == "Свободен" ? "🟢" :
                                       t.Status == "Занят" ? "🔴" : "🟠";
                    dgvReport.Rows.Add(
                        $"#{t.TableNumber}",
                        $"{t.Capacity} чел.",
                        $"{statusIcon} {t.Status}",
                        t.TodayReservations,
                        t.TotalReservations
                    );
                    totalToday += t.TodayReservations;
                    totalAll += t.TotalReservations;
                }

                dgvReport.Rows.Add("━━━━━━━━━━━━━", "━━━━━━━━━━━━━", "━━━━━━━━━━━━━━", "━━━━━━━━━━━━━━", "━━━━━━━━━━━━━━");
                dgvReport.Rows.Add("ИТОГО:", "", "", totalToday, totalAll);

                lblTotal.Text = $"📊 Всего броней сегодня: {totalToday} | Всего броней: {totalAll}";
                lblTotal.ForeColor = Color.FromArgb(155, 89, 182);
            }
            catch (Exception ex)
            {
                dgvReport.Rows.Add($"Ошибка: {ex.Message}", "", "", "", "");
            }
        }

        private void GenerateReservationsReport(DateTime startDate, DateTime endDate)
        {
            dgvReport.Columns.Add("Date", "Дата");
            dgvReport.Columns.Add("Day", "День недели");
            dgvReport.Columns.Add("Reservations", "Кол-во броней");
            dgvReport.Columns.Add("Guests", "Всего гостей");
            dgvReport.Columns.Add("Status", "Статус");

            try
            {
                var reservations = _context.Reservations
                    .Where(r => r.ReservationDate >= startDate.Date && r.ReservationDate <= endDate.Date)
                    .GroupBy(r => r.ReservationDate)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count(),
                        Guests = g.Sum(r => r.GuestCount),
                        Active = g.Count(r => r.Status == "Активна"),
                        Completed = g.Count(r => r.Status == "Завершена"),
                        Cancelled = g.Count(r => r.Status == "Отменена")
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                if (reservations == null || reservations.Count == 0)
                {
                    dgvReport.Rows.Add("Нет броней", "", "", "", "");
                    return;
                }

                int totalReservations = 0;
                int totalGuests = 0;

                foreach (var r in reservations)
                {
                    string dayOfWeek = r.Date.DayOfWeek switch
                    {
                        DayOfWeek.Monday => "Пн",
                        DayOfWeek.Tuesday => "Вт",
                        DayOfWeek.Wednesday => "Ср",
                        DayOfWeek.Thursday => "Чт",
                        DayOfWeek.Friday => "Пт",
                        DayOfWeek.Saturday => "Сб",
                        DayOfWeek.Sunday => "Вс",
                        _ => ""
                    };

                    string status = $"{r.Active} акт. / {r.Completed} зав. / {r.Cancelled} отм.";

                    dgvReport.Rows.Add(
                        r.Date.ToString("dd.MM.yyyy"),
                        dayOfWeek,
                        r.Count,
                        r.Guests,
                        status
                    );
                    totalReservations += r.Count;
                    totalGuests += r.Guests;
                }

                dgvReport.Rows.Add("━━━━━━━━━━━━", "━━━━━━━━━━━━", "━━━━━━━━━━━━", "━━━━━━━━━━━━", "━━━━━━━━━━━━");
                dgvReport.Rows.Add("ИТОГО:", "", totalReservations, totalGuests, "");

                lblTotal.Text = $"📊 Всего броней: {totalReservations} | Всего гостей: {totalGuests}";
                lblTotal.ForeColor = Color.FromArgb(46, 204, 113);
            }
            catch (Exception ex)
            {
                dgvReport.Rows.Add($"Ошибка: {ex.Message}", "", "", "", "");
            }
        }
    }
}