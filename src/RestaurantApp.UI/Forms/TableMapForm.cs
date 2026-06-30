using RestaurantApp.Core.Models;

namespace RestaurantApp.UI.Forms
{
    public partial class TableMapForm : Form
    {
        private List<Table> _tables;
        private DateTime _selectedDate;
        private DateTimePicker dtpDate = new();
        private FlowLayoutPanel flowPanel = new();

        public TableMapForm()
        {
            _selectedDate = DateTime.Now;
            InitializeComponent();
            LoadTables();
            DrawTables();
        }

        private void InitializeComponent()
        {
            this.Text = "Схема зала - Ресторан";
            this.Size = new Size(900, 650);
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

            // Выбор даты
            var lblDate = new Label
            {
                Text = "Дата:",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 70),
                AutoSize = true
            };

            dtpDate = new DateTimePicker
            {
                Location = new Point(120, 68),
                Size = new Size(200, 30),
                Format = DateTimePickerFormat.Short,
                Value = _selectedDate
            };
            dtpDate.ValueChanged += (s, e) =>
            {
                _selectedDate = dtpDate.Value;
                DrawTables();
            };

            // Легенда
            var legendPanel = new Panel
            {
                Location = new Point(20, 110),
                Size = new Size(850, 40),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            AddLegendItem(legendPanel, Color.Green, "🟢 Свободен", 10);
            AddLegendItem(legendPanel, Color.Red, "🔴 Занят", 130);
            AddLegendItem(legendPanel, Color.Orange, "🟠 Забронирован", 250);

            // Панель для столов
            flowPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 160),
                Size = new Size(850, 430),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10)
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblDate);
            this.Controls.Add(dtpDate);
            this.Controls.Add(legendPanel);
            this.Controls.Add(flowPanel);
        }

        private void AddLegendItem(Panel parent, Color color, string text, int x)
        {
            var panel = new Panel
            {
                Location = new Point(x, 5),
                Size = new Size(100, 30)
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

        private void LoadTables()
        {
            _tables = new List<Table>();
            for (int i = 1; i <= 10; i++)
            {
                var statuses = new[] { "Свободен", "Занят", "Забронирован" };
                var random = new Random();
                _tables.Add(new Table
                {
                    TableID = i,
                    TableNumber = i,
                    Capacity = i <= 8 ? 4 : 2,
                    Status = statuses[random.Next(0, 3)]
                });
            }
        }

        private void DrawTables()
        {
            flowPanel.Controls.Clear();

            foreach (var table in _tables)
            {
                var color = GetStatusColor(table.Status);
                var panel = CreateTablePanel(table, color);
                flowPanel.Controls.Add(panel);
            }
        }

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "Свободен" => Color.FromArgb(46, 204, 113),
                "Занят" => Color.FromArgb(231, 76, 60),
                "Забронирован" => Color.FromArgb(241, 196, 15),
                _ => Color.Gray
            };
        }

        private Panel CreateTablePanel(Table table, Color color)
        {
            var panel = new Panel
            {
                Width = 140,
                Height = 140,
                BackColor = color,
                Margin = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };

            var lblNumber = new Label
            {
                Text = $"Стол {table.TableNumber}",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 0, 0, 50)
            };

            var lblCapacity = new Label
            {
                Text = $"👤 {table.Capacity} чел.",
                Dock = DockStyle.Bottom,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            var lblStatus = new Label
            {
                Text = table.Status,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            panel.Controls.Add(lblStatus);
            panel.Controls.Add(lblCapacity);
            panel.Controls.Add(lblNumber);

            panel.Click += (s, e) =>
            {
                MessageBox.Show($"Стол #{table.TableNumber}\nСтатус: {table.Status}\nВместимость: {table.Capacity} чел.",
                    "Информация о столе", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            return panel;
        }
    }
}