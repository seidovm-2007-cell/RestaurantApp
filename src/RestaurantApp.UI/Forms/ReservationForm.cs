using RestaurantApp.Core.Models;
using RestaurantApp.Services;

namespace RestaurantApp.UI.Forms
{
    public partial class ReservationForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly User _currentUser;
        private readonly OrderService _orderService;
        private List<Table> _tables = new();
        private List<MenuItem> _menuItems = new();

        private ComboBox cmbTable = new();
        private DateTimePicker dtpDate = new();
        private DateTimePicker dtpTime = new();
        private NumericUpDown nudGuests = new();
        private TextBox txtClient = new();
        private Button btnCreate = new();
        private Button btnCancel = new();
        private Button btnOrder = new();
        private DataGridView dgvReservations = new();
        private DataGridView dgvMenu = new();
        private Label lblTotal = new();
        private ComboBox cmbMenuItem = new();
        private NumericUpDown nudQuantity = new();
        private Button btnAddToOrder = new();
        private List<OrderItem> _orderItems = new();

        public ReservationForm(IServiceProvider serviceProvider, User currentUser)
        {
            _serviceProvider = serviceProvider;
            _currentUser = currentUser;
            _orderService = (OrderService)serviceProvider.GetService(typeof(OrderService))!;
            _orderItems = new List<OrderItem>();
            InitializeComponent();
            LoadTables();
            LoadMenuItems();
            LoadReservations();
        }

        private void InitializeComponent()
        {
            this.Text = "Бронирования - Ресторан";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            int y = 20;

            // ============================================================
            // ЗАГОЛОВОК
            // ============================================================
            var lblTitle = new Label
            {
                Text = "📅 Бронирование столика",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            y += 50;

            // ============================================================
            // ПАНЕЛЬ БРОНИРОВАНИЯ
            // ============================================================
            var groupBox = new GroupBox
            {
                Text = "Забронировать столик",
                Location = new Point(20, y),
                Size = new Size(450, 300),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            int gy = 30;

            // Стол
            var lblTable = new Label
            {
                Text = "Стол:",
                Location = new Point(20, gy),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(lblTable);

            cmbTable = new ComboBox
            {
                Location = new Point(110, gy),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(cmbTable);
            gy += 35;

            // Дата
            var lblDate = new Label
            {
                Text = "Дата:",
                Location = new Point(20, gy),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(lblDate);

            dtpDate = new DateTimePicker
            {
                Location = new Point(110, gy),
                Size = new Size(300, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(1),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(dtpDate);
            gy += 35;

            // Время
            var lblTime = new Label
            {
                Text = "Время:",
                Location = new Point(20, gy),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(lblTime);

            dtpTime = new DateTimePicker
            {
                Location = new Point(110, gy),
                Size = new Size(300, 25),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Now.AddHours(2),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(dtpTime);
            gy += 35;

            // Количество гостей
            var lblGuests = new Label
            {
                Text = "Гостей:",
                Location = new Point(20, gy),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(lblGuests);

            nudGuests = new NumericUpDown
            {
                Location = new Point(110, gy),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 4,
                Value = 2,
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(nudGuests);
            gy += 35;

            // Клиент (только для администратора)
            if (_currentUser.RoleID == 5) // Администратор
            {
                var lblClient = new Label
                {
                    Text = "Клиент:",
                    Location = new Point(20, gy),
                    Size = new Size(80, 25),
                    Font = new Font("Segoe UI", 10)
                };
                groupBox.Controls.Add(lblClient);

                txtClient = new TextBox
                {
                    Location = new Point(110, gy),
                    Size = new Size(300, 25),
                    Font = new Font("Segoe UI", 10),
                    Text = _currentUser.Login
                };
                groupBox.Controls.Add(txtClient);
                gy += 35;
            }

            // Кнопка бронирования
            btnCreate = new Button
            {
                Text = "✅ Забронировать",
                Location = new Point(100, gy),
                Size = new Size(250, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCreate.Click += BtnCreate_Click!;
            groupBox.Controls.Add(btnCreate);

            this.Controls.Add(groupBox);
            y += 320;

            // ============================================================
            // СПИСОК АКТИВНЫХ БРОНЕЙ
            // ============================================================
            var lblReservations = new Label
            {
                Text = "Активные бронирования:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblReservations);
            y += 30;

            dgvReservations = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(1050, 150),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvReservations.Columns.Add("Id", "№");
            dgvReservations.Columns.Add("Table", "Стол");
            dgvReservations.Columns.Add("Client", "Клиент");
            dgvReservations.Columns.Add("Date", "Дата");
            dgvReservations.Columns.Add("Time", "Время");
            dgvReservations.Columns.Add("Guests", "Гостей");
            dgvReservations.Columns.Add("Status", "Статус");

            this.Controls.Add(dgvReservations);
            y += 170;

            // Кнопка отмены брони
            btnCancel = new Button
            {
                Text = "❌ Отменить выбранную бронь",
                Location = new Point(20, y),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCancel.Click += BtnCancel_Click!;
            this.Controls.Add(btnCancel);

            // ============================================================
            // МЕНЮ ДЛЯ ЗАКАЗА
            // ============================================================
            if (_currentUser.RoleID == 5 || _currentUser.RoleID == 6) // Админ или официант
            {
                var lblMenu = new Label
                {
                    Text = "🍕 Добавить блюдо к брони:",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(44, 62, 80),
                    Location = new Point(480, 320),
                    AutoSize = true
                };
                this.Controls.Add(lblMenu);

                var lblDish = new Label
                {
                    Text = "Блюдо:",
                    Location = new Point(480, 360),
                    Size = new Size(80, 25),
                    Font = new Font("Segoe UI", 10)
                };
                this.Controls.Add(lblDish);

                cmbMenuItem = new ComboBox
                {
                    Location = new Point(560, 358),
                    Size = new Size(200, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10)
                };
                this.Controls.Add(cmbMenuItem);

                var lblQty = new Label
                {
                    Text = "Кол-во:",
                    Location = new Point(480, 395),
                    Size = new Size(80, 25),
                    Font = new Font("Segoe UI", 10)
                };
                this.Controls.Add(lblQty);

                nudQuantity = new NumericUpDown
                {
                    Location = new Point(560, 393),
                    Size = new Size(80, 25),
                    Minimum = 1,
                    Maximum = 99,
                    Value = 1,
                    Font = new Font("Segoe UI", 10)
                };
                this.Controls.Add(nudQuantity);

                btnAddToOrder = new Button
                {
                    Text = "➕ Добавить к заказу",
                    Location = new Point(660, 390),
                    Size = new Size(150, 30),
                    BackColor = Color.FromArgb(52, 152, 219),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                btnAddToOrder.Click += BtnAddToOrder_Click!;
                this.Controls.Add(btnAddToOrder);

                // Список заказа
                var lblOrderItems = new Label
                {
                    Text = "Заказ:",
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(480, 430),
                    AutoSize = true
                };
                this.Controls.Add(lblOrderItems);

                dgvMenu = new DataGridView
                {
                    Location = new Point(480, 455),
                    Size = new Size(500, 150),
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    RowHeadersVisible = false
                };

                dgvMenu.Columns.Add("Id", "ID");
                dgvMenu.Columns.Add("Name", "Блюдо");
                dgvMenu.Columns.Add("Qty", "Кол-во");
                dgvMenu.Columns.Add("Price", "Цена");

                dgvMenu.Columns["Id"].Width = 40;
                dgvMenu.Columns["Name"].Width = 200;
                dgvMenu.Columns["Qty"].Width = 60;
                dgvMenu.Columns["Price"].Width = 80;

                this.Controls.Add(dgvMenu);

                // Итоговая сумма
                lblTotal = new Label
                {
                    Text = "Итого: 0.00 ₽",
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(231, 76, 60),
                    Location = new Point(480, 620),
                    AutoSize = true
                };
                this.Controls.Add(lblTotal);

                btnOrder = new Button
                {
                    Text = "📋 Оформить заказ",
                    Location = new Point(480, 650),
                    Size = new Size(200, 35),
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold)
                };
                btnOrder.Click += BtnOrder_Click!;
                this.Controls.Add(btnOrder);
            }
        }

        // ============================================================
        // МЕТОДЫ ЗАГРУЗКИ ДАННЫХ
        // ============================================================

        private void LoadTables()
        {
            for (int i = 1; i <= 10; i++)
            {
                var table = new Table { TableID = i, TableNumber = i, Capacity = i <= 8 ? 4 : 2, Status = "Свободен" };
                _tables.Add(table);
                cmbTable.Items.Add($"Стол #{i} ({table.Capacity} чел.)");
            }
            cmbTable.SelectedIndex = 0;
        }

        private void LoadMenuItems()
        {
            _menuItems = new List<MenuItem>
            {
                new MenuItem { MenuItemID = 1, Name = "Борщ", Price = 250 },
                new MenuItem { MenuItemID = 2, Name = "Стейк", Price = 950 },
                new MenuItem { MenuItemID = 3, Name = "Цезарь", Price = 320 },
                new MenuItem { MenuItemID = 4, Name = "Чай", Price = 80 },
                new MenuItem { MenuItemID = 5, Name = "Тирамису", Price = 290 }
            };

            if (cmbMenuItem != null)
            {
                cmbMenuItem.DataSource = _menuItems;
                cmbMenuItem.DisplayMember = "Name";
                cmbMenuItem.ValueMember = "MenuItemID";
            }
        }

        private void LoadReservations()
        {
            dgvReservations.Rows.Clear();
            // Заглушка - в реальном проекте загружать из БД
            for (int i = 1; i <= 3; i++)
            {
                dgvReservations.Rows.Add(
                    i,
                    i + 1,
                    $"Клиент {i}",
                    DateTime.Now.AddDays(i).ToShortDateString(),
                    $"{18 + i}:00",
                    2 + i % 2,
                    "Активна"
                );
            }
        }

        // ============================================================
        // БРОНИРОВАНИЕ
        // ============================================================

        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            try
            {
                int tableIndex = cmbTable.SelectedIndex;
                if (tableIndex < 0)
                {
                    MessageBox.Show("Выберите столик!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var table = _tables[tableIndex];
                var date = dtpDate.Value.ToShortDateString();
                var time = dtpTime.Value.ToShortTimeString();
                int guests = (int)nudGuests.Value;
                string client = _currentUser.RoleID == 5 ? txtClient.Text.Trim() : _currentUser.Login;

                if (string.IsNullOrWhiteSpace(client))
                {
                    MessageBox.Show("Введите имя клиента!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка доступности стола (заглушка)
                Random rnd = new Random();
                if (rnd.Next(0, 5) == 0)
                {
                    MessageBox.Show($"Стол #{table.TableNumber} уже забронирован на это время!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dgvReservations.Rows.Add(
                    dgvReservations.Rows.Count + 1,
                    $"#{table.TableNumber}",
                    client,
                    date,
                    time,
                    guests,
                    "Активна"
                );

                MessageBox.Show($"✅ Бронь создана!\n" +
                    $"Стол: #{table.TableNumber}\n" +
                    $"Клиент: {client}\n" +
                    $"Дата: {date} {time}\n" +
                    $"Гостей: {guests}",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (_currentUser.RoleID != 5)
                    txtClient.Text = "";
                nudGuests.Value = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ОТМЕНА БРОНИ
        // ============================================================

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvReservations.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите бронь для отмены!", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show("Отменить выбранную бронь?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int rowIndex = dgvReservations.SelectedRows[0].Index;
                    dgvReservations.Rows.RemoveAt(rowIndex);

                    MessageBox.Show("✅ Бронь отменена!", "Успешно",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ДОБАВЛЕНИЕ БЛЮДА К ЗАКАЗУ
        // ============================================================

        private void BtnAddToOrder_Click(object? sender, EventArgs e)
        {
            try
            {
                var menuItem = cmbMenuItem.SelectedItem as MenuItem;
                if (menuItem == null)
                {
                    MessageBox.Show("Выберите блюдо!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int quantity = (int)nudQuantity.Value;

                var orderItem = new OrderItem
                {
                    OrderItemID = _orderItems.Count + 1,
                    MenuItemID = menuItem.MenuItemID,
                    Quantity = quantity,
                    UnitPrice = menuItem.Price,
                    MenuItem = menuItem
                };

                _orderItems.Add(orderItem);
                RefreshOrderGrid();
                UpdateTotal();

                MessageBox.Show($"✅ Блюдо '{menuItem.Name}' x{quantity} добавлено к заказу!",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshOrderGrid()
        {
            if (dgvMenu == null) return;
            dgvMenu.Rows.Clear();
            foreach (var item in _orderItems)
            {
                dgvMenu.Rows.Add(
                    item.OrderItemID,
                    item.MenuItem?.Name ?? "Блюдо",
                    item.Quantity,
                    item.UnitPrice
                );
            }
        }

        private void UpdateTotal()
        {
            if (lblTotal == null) return;
            decimal total = 0;
            foreach (var item in _orderItems)
            {
                total += item.Quantity * item.UnitPrice;
            }
            lblTotal.Text = $"Итого: {total:F2} ₽";
        }

        // ============================================================
        // ОФОРМЛЕНИЕ ЗАКАЗА
        // ============================================================

        private void BtnOrder_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_orderItems.Count == 0)
                {
                    MessageBox.Show("Добавьте хотя бы одно блюдо в заказ!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal total = 0;
                foreach (var item in _orderItems)
                {
                    total += item.Quantity * item.UnitPrice;
                }

                var result = MessageBox.Show($"Оформить заказ на сумму {total:F2} ₽?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show($"✅ Заказ оформлен!\nСумма: {total:F2} ₽\n" +
                        $"Блюда: {_orderItems.Count} позиций",
                        "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _orderItems.Clear();
                    RefreshOrderGrid();
                    UpdateTotal();
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