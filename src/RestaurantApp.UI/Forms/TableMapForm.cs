using RestaurantApp.Core.Models;
using RestaurantApp.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.UI.Forms
{
    public partial class TableMapForm : Form
    {
        private readonly AppDbContext _context;
        private readonly User _currentUser;
        private List<Table> _tables = new();
        private DateTimePicker dtpDate;
        private FlowLayoutPanel flowPanel;
        private Label lblInfo;
        private Button btnRefresh;
        private Button btnClose;

        public TableMapForm(User currentUser)
        {
            _currentUser = currentUser;
            _context = new AppDbContext();
            InitializeComponent();  // ← Вызывает метод из .Designer.cs
            SetupForm();            // ← Дополнительная настройка
            LoadTables();
            DrawTables();
        }

        // ============================================================
        // ВСЯ ЛОГИКА ПОСЛЕ ИНИЦИАЛИЗАЦИИ
        // ============================================================
        private void SetupForm()
        {
            this.Text = "🪑 Схема зала - Ресторан";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "🪑 Схема зала",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Панель управления
            var pnlControls = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(940, 50),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblDate = new Label
            {
                Text = "📅 Дата:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(15, 13),
                AutoSize = true
            };
            pnlControls.Controls.Add(lblDate);

            dtpDate = new DateTimePicker
            {
                Location = new Point(80, 10),
                Size = new Size(160, 30),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 11),
                MinDate = DateTime.Today,
                MaxDate = DateTime.Today.AddDays(30)
            };
            dtpDate.ValueChanged += DtpDate_ValueChanged!;
            pnlControls.Controls.Add(dtpDate);

            lblInfo = new Label
            {
                Text = "📌 Выберите дату и нажмите на стол",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(270, 15),
                AutoSize = true
            };
            pnlControls.Controls.Add(lblInfo);

            btnRefresh = new Button
            {
                Text = "🔄 Обновить",
                Location = new Point(650, 6),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnRefresh.Click += BtnRefresh_Click!;
            pnlControls.Controls.Add(btnRefresh);

            btnClose = new Button
            {
                Text = "✖ Закрыть",
                Location = new Point(760, 6),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => this.Close();
            pnlControls.Controls.Add(btnClose);

            this.Controls.Add(pnlControls);

            // Легенда
            int y = 120;
            var pnlLegend = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(940, 40),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            AddLegendItem(pnlLegend, Color.FromArgb(46, 204, 113), "🟢 Свободен", 20);
            AddLegendItem(pnlLegend, Color.FromArgb(231, 76, 60), "🔴 Занят", 180);
            AddLegendItem(pnlLegend, Color.FromArgb(241, 196, 15), "🟠 Забронирован", 340);
            AddLegendItem(pnlLegend, Color.FromArgb(52, 152, 219), "🔵 Нажмите для деталей", 500);

            this.Controls.Add(pnlLegend);

            // Панель со столами
            y += 60;
            flowPanel = new FlowLayoutPanel
            {
                Location = new Point(20, y),
                Size = new Size(940, 460),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10),
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.Add(flowPanel);

            UpdateDateInfo();
        }

        private void AddLegendItem(Panel parent, Color color, string text, int x)
        {
            var panel = new Panel
            {
                Location = new Point(x, 5),
                Size = new Size(150, 30)
            };

            var box = new Panel
            {
                Location = new Point(0, 5),
                Size = new Size(20, 20),
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle
            };

            var label = new Label
            {
                Text = text,
                Location = new Point(25, 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            panel.Controls.Add(box);
            panel.Controls.Add(label);
            parent.Controls.Add(panel);
        }

        private void UpdateDateInfo()
        {
            var selectedDate = dtpDate.Value.Date;
            var today = DateTime.Today;

            if (selectedDate < today)
            {
                lblInfo.Text = "⛔ ПРОШЛАЯ ДАТА: только просмотр";
                lblInfo.ForeColor = Color.Gray;
            }
            else if (selectedDate == today)
            {
                lblInfo.Text = "✅ СЕГОДНЯ: можно просмотреть и забронировать";
                lblInfo.ForeColor = Color.FromArgb(46, 204, 113);
            }
            else
            {
                lblInfo.Text = "📅 БУДУЩАЯ ДАТА: можно просмотреть и забронировать";
                lblInfo.ForeColor = Color.FromArgb(52, 152, 219);
            }
        }

        private void LoadTables()
        {
            try
            {
                _tables = _context.Tables.ToList();

                if (_tables == null || _tables.Count == 0)
                {
                    CreateDefaultTables();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки столов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CreateDefaultTables();
            }
        }

        private void CreateDefaultTables()
        {
            _tables = new List<Table>();
            for (int i = 1; i <= 10; i++)
            {
                _tables.Add(new Table
                {
                    TableID = i,
                    TableNumber = i,
                    Capacity = i <= 8 ? 4 : 2,
                    Status = "Свободен"
                });
            }
        }

        private void DtpDate_ValueChanged(object? sender, EventArgs e)
        {
            DrawTables();
            UpdateDateInfo();
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadTables();
            DrawTables();
            UpdateDateInfo();
        }

        private void DrawTables()
        {
            flowPanel.Controls.Clear();

            var selectedDate = dtpDate.Value.Date;
            var today = DateTime.Today;
            bool isPastDate = selectedDate < today;

            foreach (var table in _tables)
            {
                var reservation = _context.Reservations
                    .FirstOrDefault(r => r.TableID == table.TableID &&
                                         r.ReservationDate == selectedDate &&
                                         r.Status != "Отменена");

                string displayStatus;
                Color bgColor;

                if (isPastDate)
                {
                    bgColor = Color.Gray;
                    displayStatus = "⚪ Недоступно";
                }
                else if (reservation != null)
                {
                    displayStatus = reservation.Status == "Занят" ? "🔴 Занят" : "🟠 Забронирован";
                    bgColor = reservation.Status == "Занят" ? Color.FromArgb(231, 76, 60) : Color.FromArgb(241, 196, 15);
                }
                else
                {
                    displayStatus = "🟢 Свободен";
                    bgColor = Color.FromArgb(46, 204, 113);
                }

                var panel = CreateTablePanel(table, bgColor, displayStatus, isPastDate, reservation);
                flowPanel.Controls.Add(panel);
            }
        }

        private Panel CreateTablePanel(Table table, Color color, string statusText, bool isPastDate, Reservation? reservation)
        {
            var panel = new Panel
            {
                Width = 170,
                Height = 170,
                BackColor = color,
                Margin = new Padding(12),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = isPastDate ? Cursors.Default : Cursors.Hand
            };

            var lblNumber = new Label
            {
                Text = $"Стол {table.TableNumber}",
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 0, 0, 50)
            };

            var lblCapacity = new Label
            {
                Text = $"👤 {table.Capacity} чел.",
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11)
            };

            var lblStatus = new Label
            {
                Text = statusText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold)
            };

            if (reservation != null)
            {
                var lblTime = new Label
                {
                    Text = $"⏰ {reservation.ReservationTime:hh\\:mm} - {reservation.GuestCount} чел.",
                    Dock = DockStyle.Bottom,
                    Height = 25,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9),
                    BackColor = Color.FromArgb(0, 0, 0, 40)
                };
                panel.Controls.Add(lblTime);
            }
            else if (!isPastDate)
            {
                var lblHint = new Label
                {
                    Text = "🔄 Нажмите для просмотра",
                    Dock = DockStyle.Bottom,
                    Height = 25,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9),
                    BackColor = Color.FromArgb(0, 0, 0, 30)
                };
                panel.Controls.Add(lblHint);
            }
            else
            {
                var lblHint = new Label
                {
                    Text = "⛔ Только просмотр",
                    Dock = DockStyle.Bottom,
                    Height = 25,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(200, 200, 200),
                    Font = new Font("Segoe UI", 9),
                    BackColor = Color.FromArgb(0, 0, 0, 30)
                };
                panel.Controls.Add(lblHint);
            }

            panel.Controls.Add(lblStatus);
            panel.Controls.Add(lblCapacity);
            panel.Controls.Add(lblNumber);

            if (!isPastDate)
            {
                panel.Click += (s, e) => Table_Click(table, reservation);
            }

            return panel;
        }

        private void Table_Click(Table table, Reservation? reservation)
        {
            try
            {
                var selectedDate = dtpDate.Value.Date;

                if (selectedDate < DateTime.Today)
                {
                    MessageBox.Show("⛔ Нельзя просматривать прошедшие даты!", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}