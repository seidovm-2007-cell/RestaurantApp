namespace RestaurantApp.UI.Forms
{
    public partial class ReportsForm : Form
    {
        private DataGridView dgvReport = new();
        private ComboBox cmbReportType = new();
        private Button btnGenerate = new();

        public ReportsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Отчеты и статистика - Ресторан";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "📊 Отчеты и статистика",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Выбор типа отчета
            var lblType = new Label
            {
                Text = "Тип отчета:",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 70),
                AutoSize = true
            };

            cmbReportType = new ComboBox
            {
                Location = new Point(150, 68),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            cmbReportType.Items.AddRange(new object[]
            {
                "Продажи по категориям",
                "Работа официантов",
                "Популярность блюд",
                "Занятость столов"
            });
            cmbReportType.SelectedIndex = 0;

            btnGenerate = new Button
            {
                Text = "📊 Сформировать",
                Location = new Point(420, 65),
                Size = new Size(150, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnGenerate.Click += BtnGenerate_Click!;

            // DataGridView для отчета
            dgvReport = new DataGridView
            {
                Location = new Point(20, 120),
                Size = new Size(950, 480),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblType);
            this.Controls.Add(cmbReportType);
            this.Controls.Add(btnGenerate);
            this.Controls.Add(dgvReport);
        }

        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            try
            {
                dgvReport.Columns.Clear();
                dgvReport.Rows.Clear();

                string reportType = cmbReportType.SelectedItem?.ToString() ?? "";

                switch (reportType)
                {
                    case "Продажи по категориям":
                        GenerateSalesReport();
                        break;
                    case "Работа официантов":
                        GenerateWaiterReport();
                        break;
                    case "Популярность блюд":
                        GeneratePopularityReport();
                        break;
                    case "Занятость столов":
                        GenerateTableOccupancyReport();
                        break;
                    default:
                        MessageBox.Show("Выберите тип отчета", "Информация",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateSalesReport()
        {
            dgvReport.Columns.Add("Category", "Категория");
            dgvReport.Columns.Add("Dish", "Блюдо");
            dgvReport.Columns.Add("Quantity", "Кол-во продаж");
            dgvReport.Columns.Add("Revenue", "Выручка");

            var data = new (string category, string dish, int qty, decimal rev)[]
            {
                ("Супы", "Борщ", 45, 11250),
                ("Супы", "Куриный суп", 30, 6000),
                ("Горячие", "Стейк", 20, 19000),
                ("Горячие", "Паста", 35, 13300),
                ("Салаты", "Цезарь", 40, 12800),
                ("Напитки", "Чай", 150, 12000),
                ("Десерты", "Тирамису", 25, 7250)
            };

            foreach (var item in data)
            {
                dgvReport.Rows.Add(item.category, item.dish, item.qty, $"{item.rev:F2} ₽");
            }
        }

        private void GenerateWaiterReport()
        {
            dgvReport.Columns.Add("Waiter", "Официант");
            dgvReport.Columns.Add("Orders", "Заказов");
            dgvReport.Columns.Add("Payments", "Оплат");
            dgvReport.Columns.Add("Revenue", "Выручка");

            var data = new (string name, int orders, int payments, decimal rev)[]
            {
                ("Петров П.", 15, 12, 45000),
                ("Сидоров С.", 10, 8, 32000),
                ("Кузнецов К.", 8, 7, 28000)
            };

            foreach (var item in data)
            {
                dgvReport.Rows.Add(item.name, item.orders, item.payments, $"{item.rev:F2} ₽");
            }
        }

        private void GeneratePopularityReport()
        {
            dgvReport.Columns.Add("Dish", "Блюдо");
            dgvReport.Columns.Add("Orders", "Кол-во заказов");
            dgvReport.Columns.Add("Rating", "Рейтинг");

            var data = new (string dish, int orders, string rating)[]
            {
                ("Борщ", 45, "★★★★★"),
                ("Чай", 40, "★★★★☆"),
                ("Стейк", 35, "★★★★☆"),
                ("Цезарь", 30, "★★★☆☆"),
                ("Тирамису", 25, "★★★☆☆")
            };

            foreach (var item in data)
            {
                dgvReport.Rows.Add(item.dish, item.orders, item.rating);
            }
        }

        private void GenerateTableOccupancyReport()
        {
            dgvReport.Columns.Add("Table", "Стол");
            dgvReport.Columns.Add("Status", "Статус");
            dgvReport.Columns.Add("BusyTime", "Время занятости");
            dgvReport.Columns.Add("Reservations", "Броней");

            var data = new (int table, string status, string time, int reservations)[]
            {
                (1, "Свободен", "0 ч", 0),
                (2, "Занят", "2.5 ч", 0),
                (3, "Свободен", "0 ч", 0),
                (4, "Забронирован", "0 ч", 1),
                (5, "Занят", "1.5 ч", 0),
                (6, "Свободен", "0 ч", 0),
                (7, "Свободен", "0 ч", 0),
                (8, "Забронирован", "0 ч", 2),
                (9, "Занят", "3 ч", 0),
                (10, "Свободен", "0 ч", 0)
            };

            foreach (var item in data)
            {
                dgvReport.Rows.Add($"#{item.table}", item.status, item.time, item.reservations);
            }
        }
    }
}