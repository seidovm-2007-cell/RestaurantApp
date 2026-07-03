using RestaurantApp.Core.Models;
using RestaurantApp.Services;
using RestaurantApp.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace RestaurantApp.UI.Forms
{
    public partial class OrderForm : Form
    {
        private readonly OrderService _orderService;
        private readonly User _currentUser;
        private readonly AppDbContext _context;
        private Order _currentOrder = new Order();
        private List<MenuItem> _menuItems = new List<MenuItem>();
        private List<Table> _tables = new List<Table>();

        private ComboBox cmbTables = new();
        private DateTimePicker dtpDate = new();
        private DateTimePicker dtpTime = new();
        private NumericUpDown nudGuests = new();
        private Button btnCheckAvailability = new();
        private Button btnCreateReservation = new();
        private DataGridView dgvOrderItems = new();
        private ComboBox cmbDishes = new();
        private NumericUpDown nudQuantity = new();
        private Button btnAddDish = new();
        private Button btnRemoveDish = new();
        private Button btnConfirmOrder = new();
        private Label lblTotal = new();
        private Label lblStatus = new();
        private Label lblReservationInfo = new();
        private Label lblOrderInfo = new();

        public OrderForm(OrderService orderService, User currentUser)
        {
            _orderService = orderService;
            _currentUser = currentUser;
            _context = new AppDbContext();
            InitializeComponent();
            LoadMenuItems();
            LoadTables();
            InitializeOrder();
        }

        private void InitializeComponent()
        {
            this.Text = "📋 Создание нового заказа";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            int y = 20;

            var lblTitle = new Label
            {
                Text = "📋 Создание нового заказа",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            y += 50;

            var groupReservation = new GroupBox
            {
                Text = "🪑 Бронирование стола",
                Location = new Point(20, y),
                Size = new Size(500, 200),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            int gy = 25;

            var lblTable = new Label
            {
                Text = "Стол:",
                Location = new Point(15, gy),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupReservation.Controls.Add(lblTable);

            cmbTables = new ComboBox
            {
                Location = new Point(100, gy),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cmbTables.SelectedIndexChanged += CmbTables_SelectedIndexChanged!;
            groupReservation.Controls.Add(cmbTables);
            gy += 30;

            var lblDate = new Label
            {
                Text = "Дата:",
                Location = new Point(15, gy),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupReservation.Controls.Add(lblDate);

            dtpDate = new DateTimePicker
            {
                Location = new Point(100, gy),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today,
                Font = new Font("Segoe UI", 10),
                MinDate = DateTime.Today,
                MaxDate = DateTime.Today.AddDays(30)
            };
            dtpDate.ValueChanged += DtpDate_ValueChanged!;
            groupReservation.Controls.Add(dtpDate);
            gy += 30;

            var lblTime = new Label
            {
                Text = "Время:",
                Location = new Point(15, gy),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupReservation.Controls.Add(lblTime);

            dtpTime = new DateTimePicker
            {
                Location = new Point(100, gy),
                Size = new Size(150, 25),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Now.AddHours(1),
                Font = new Font("Segoe UI", 10)
            };
            groupReservation.Controls.Add(dtpTime);
            gy += 30;

            var lblGuests = new Label
            {
                Text = "Гостей:",
                Location = new Point(15, gy),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupReservation.Controls.Add(lblGuests);

            nudGuests = new NumericUpDown
            {
                Location = new Point(100, gy),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 4,
                Value = 2,
                Font = new Font("Segoe UI", 10)
            };
            groupReservation.Controls.Add(nudGuests);
            gy += 30;

            lblReservationInfo = new Label
            {
                Text = "📌 Выберите стол и дату",
                Location = new Point(15, gy),
                Size = new Size(470, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            groupReservation.Controls.Add(lblReservationInfo);

            btnCheckAvailability = new Button
            {
                Text = "🔍 Проверить доступность",
                Location = new Point(15, gy + 30),
                Size = new Size(160, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCheckAvailability.Click += BtnCheckAvailability_Click!;
            groupReservation.Controls.Add(btnCheckAvailability);

            btnCreateReservation = new Button
            {
                Text = "✅ Создать бронь и заказ",
                Location = new Point(185, gy + 30),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Enabled = false
            };
            btnCreateReservation.Click += BtnCreateReservation_Click!;
            groupReservation.Controls.Add(btnCreateReservation);

            this.Controls.Add(groupReservation);
            y += 220;

            lblOrderInfo = new Label
            {
                Text = "📋 Заказ: Составление",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblOrderInfo);

            lblStatus = new Label
            {
                Text = "Статус: Составление",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.Orange,
                Location = new Point(20, y + 25),
                AutoSize = true
            };
            this.Controls.Add(lblStatus);

            y += 55;

            dgvOrderItems = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(700, 250),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false
            };

            dgvOrderItems.Columns.Add("Id", "ID");
            dgvOrderItems.Columns.Add("Name", "Блюдо");
            dgvOrderItems.Columns.Add("Qty", "Кол-во");
            dgvOrderItems.Columns.Add("Price", "Цена");
            dgvOrderItems.Columns.Add("Discount", "Скидка %");
            dgvOrderItems.Columns.Add("Total", "Сумма");
            dgvOrderItems.Columns["Id"].Width = 40;
            dgvOrderItems.Columns["Name"].Width = 200;
            dgvOrderItems.Columns["Qty"].Width = 60;
            dgvOrderItems.Columns["Price"].Width = 80;
            dgvOrderItems.Columns["Discount"].Width = 70;
            dgvOrderItems.Columns["Total"].Width = 100;

            this.Controls.Add(dgvOrderItems);

            var groupDish = new GroupBox
            {
                Text = "🍕 Добавить блюдо",
                Location = new Point(740, y),
                Size = new Size(330, 200),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            int dy = 25;

            var lblDish = new Label
            {
                Text = "Блюдо:",
                Location = new Point(10, dy),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupDish.Controls.Add(lblDish);

            cmbDishes = new ComboBox
            {
                Location = new Point(80, dy),
                Size = new Size(230, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            groupDish.Controls.Add(cmbDishes);
            dy += 35;

            var lblQty = new Label
            {
                Text = "Кол-во:",
                Location = new Point(10, dy),
                Size = new Size(60, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupDish.Controls.Add(lblQty);

            nudQuantity = new NumericUpDown
            {
                Location = new Point(80, dy),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 99,
                Value = 1,
                Font = new Font("Segoe UI", 10)
            };
            groupDish.Controls.Add(nudQuantity);
            dy += 35;

            btnAddDish = new Button
            {
                Text = "➕ Добавить",
                Location = new Point(10, dy),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnAddDish.Click += BtnAddDish_Click!;
            groupDish.Controls.Add(btnAddDish);

            btnRemoveDish = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(160, dy),
                Size = new Size(140, 35),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnRemoveDish.Click += BtnRemoveDish_Click!;
            groupDish.Controls.Add(btnRemoveDish);

            this.Controls.Add(groupDish);
            y += 270;

            var lblTotalLabel = new Label
            {
                Text = "ИТОГО:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblTotalLabel);

            lblTotal = new Label
            {
                Text = "0.00 ₽",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(130, y - 5),
                AutoSize = true
            };
            this.Controls.Add(lblTotal);

            btnConfirmOrder = new Button
            {
                Text = "✅ Оформить заказ",
                Location = new Point(740, y - 10),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Enabled = false
            };
            btnConfirmOrder.Click += BtnConfirmOrder_Click!;
            this.Controls.Add(btnConfirmOrder);

            var btnClose = new Button
            {
                Text = "✖ Закрыть",
                Location = new Point(950, y - 10),
                Size = new Size(120, 45),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void LoadTables()
        {
            try
            {
                _tables = _context.Tables.ToList();
                cmbTables.Items.Clear();
                foreach (var table in _tables)
                {
                    string status = table.Status == "Свободен" ? "🟢" :
                                   table.Status == "Занят" ? "🔴" : "🟠";
                    cmbTables.Items.Add($"{status} Стол #{table.TableNumber} ({table.Capacity} чел.)");
                }
                if (cmbTables.Items.Count > 0)
                    cmbTables.SelectedIndex = 0;
            }
            catch
            {
                for (int i = 1; i <= 10; i++)
                {
                    cmbTables.Items.Add($"🟢 Стол #{i} ({(i <= 8 ? 4 : 2)} чел.)");
                }
                cmbTables.SelectedIndex = 0;
            }
        }

        private void LoadMenuItems()
        {
            try
            {
                _menuItems = _context.MenuItems.ToList();
                if (_menuItems.Count == 0)
                {
                    _menuItems = new List<MenuItem>
                    {
                        new MenuItem { MenuItemID = 1, Name = "Борщ", Price = 250, Category = "Супы" },
                        new MenuItem { MenuItemID = 2, Name = "Стейк", Price = 950, Category = "Горячие блюда" },
                        new MenuItem { MenuItemID = 3, Name = "Цезарь", Price = 320, Category = "Салаты" },
                        new MenuItem { MenuItemID = 4, Name = "Чай", Price = 80, Category = "Напитки" },
                        new MenuItem { MenuItemID = 5, Name = "Тирамису", Price = 290, Category = "Десерты" }
                    };
                }

                cmbDishes.DataSource = _menuItems;
                cmbDishes.DisplayMember = "Name";
                cmbDishes.ValueMember = "MenuItemID";
            }
            catch
            {
                _menuItems = new List<MenuItem>
                {
                    new MenuItem { MenuItemID = 1, Name = "Борщ", Price = 250 },
                    new MenuItem { MenuItemID = 2, Name = "Стейк", Price = 950 },
                    new MenuItem { MenuItemID = 3, Name = "Цезарь", Price = 320 },
                    new MenuItem { MenuItemID = 4, Name = "Чай", Price = 80 },
                    new MenuItem { MenuItemID = 5, Name = "Тирамису", Price = 290 }
                };
                cmbDishes.DataSource = _menuItems;
                cmbDishes.DisplayMember = "Name";
                cmbDishes.ValueMember = "MenuItemID";
            }
        }

        private void InitializeOrder()
        {
            _currentOrder = new Order
            {
                OrderID = new Random().Next(100, 999),
                OrderStatus = "Составление",
                OrderDate = DateTime.Now,
                TotalSum = 0,
                OrderItems = new List<OrderItem>()
            };
            UpdateOrderInfo();
        }

        private void CmbTables_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbTables.SelectedIndex >= 0 && cmbTables.SelectedIndex < _tables.Count)
            {
                var table = _tables[cmbTables.SelectedIndex];
                nudGuests.Maximum = table.Capacity;
                lblReservationInfo.Text = $"📌 Стол #{table.TableNumber}, вместимость: {table.Capacity} чел. Статус: {table.Status}";
            }
        }

        // ============================================================
        // ИСПРАВЛЕННЫЙ МЕТОД ОБНОВЛЕНИЯ ДАТЫ (БЕЗ ОШИБОК)
        // ============================================================
        private void DtpDate_ValueChanged(object? sender, EventArgs e)
        {
            try
            {
                var selectedDate = dtpDate.Value.Date;
                var today = DateTime.Today;

                // Если выбрана дата в прошлом — блокируем
                if (selectedDate < today)
                {
                    dtpTime.MinDate = today.AddHours(9);
                    dtpTime.MaxDate = today.AddHours(23);
                    btnCreateReservation.Enabled = false;
                    lblReservationInfo.Text = "❌ Нельзя выбрать прошедшую дату!";
                    lblReservationInfo.ForeColor = Color.Red;
                    return;
                }

                DateTime minTime;
                DateTime maxTime;

                if (selectedDate == today)
                {
                    minTime = DateTime.Now.AddMinutes(30);
                    if (minTime < selectedDate.AddHours(9))
                        minTime = selectedDate.AddHours(9);
                    maxTime = selectedDate.AddHours(23);
                }
                else
                {
                    minTime = selectedDate.AddHours(9);
                    maxTime = selectedDate.AddHours(23);
                }

                // ============================================================
                // ГЛАВНОЕ ИСПРАВЛЕНИЕ: проверяем, что minTime < maxTime
                // ============================================================
                if (minTime >= maxTime)
                {
                    minTime = selectedDate.AddHours(9);
                    maxTime = selectedDate.AddHours(23);
                }

                dtpTime.MinDate = minTime;
                dtpTime.MaxDate = maxTime;

                // Если текущее значение выходит за пределы — корректируем
                if (dtpTime.Value < minTime)
                    dtpTime.Value = minTime;
                if (dtpTime.Value > maxTime)
                    dtpTime.Value = maxTime;

                btnCreateReservation.Enabled = false;
                lblReservationInfo.Text = "📌 Выберите время для бронирования";
                lblReservationInfo.ForeColor = Color.FromArgb(100, 100, 100);
            }
            catch (Exception ex)
            {
                // В случае ошибки устанавливаем безопасные значения
                try
                {
                    var safeDate = dtpDate.Value.Date;
                    if (safeDate < DateTime.Today)
                        safeDate = DateTime.Today;

                    dtpTime.MinDate = safeDate.AddHours(9);
                    dtpTime.MaxDate = safeDate.AddHours(23);
                    btnCreateReservation.Enabled = false;
                }
                catch
                {
                    // Если совсем ничего не работает — устанавливаем сегодня
                    dtpTime.MinDate = DateTime.Today.AddHours(9);
                    dtpTime.MaxDate = DateTime.Today.AddHours(23);
                    btnCreateReservation.Enabled = false;
                }
            }
        }

        // ============================================================
        // ИСПРАВЛЕННЫЙ МЕТОД ПРОВЕРКИ ДОСТУПНОСТИ
        // ============================================================
        private void BtnCheckAvailability_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cmbTables.SelectedIndex < 0 || cmbTables.SelectedIndex >= _tables.Count)
                {
                    MessageBox.Show("Выберите стол!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var table = _tables[cmbTables.SelectedIndex];
                var selectedDate = dtpDate.Value.Date;
                var selectedTime = dtpTime.Value.TimeOfDay;

                // Проверка: прошедшая дата
                if (selectedDate < DateTime.Today)
                {
                    MessageBox.Show("❌ Нельзя бронировать на прошедшую дату!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка: нельзя бронировать на прошедшее время сегодня
                if (selectedDate == DateTime.Today && dtpTime.Value < DateTime.Now)
                {
                    MessageBox.Show("❌ Нельзя бронировать на прошедшее время!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка: рабочее время
                if (selectedTime.Hours < 9 || selectedTime.Hours >= 23)
                {
                    MessageBox.Show("❌ Ресторан работает с 9:00 до 23:00!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка: есть ли уже бронь
                var existingReservation = _context.Reservations
                    .FirstOrDefault(r => r.TableID == table.TableID &&
                                         r.ReservationDate == selectedDate &&
                                         r.ReservationTime == selectedTime &&
                                         r.Status != "Отменена");

                if (existingReservation != null)
                {
                    lblReservationInfo.Text = $"❌ Стол #{table.TableNumber} занят на {selectedDate:dd.MM.yyyy} в {selectedTime:hh\\:mm}!";
                    lblReservationInfo.ForeColor = Color.Red;
                    btnCreateReservation.Enabled = false;
                    return;
                }

                lblReservationInfo.Text = $"✅ Стол #{table.TableNumber} свободен на {selectedDate:dd.MM.yyyy} в {selectedTime:hh\\:mm}!";
                lblReservationInfo.ForeColor = Color.Green;
                btnCreateReservation.Enabled = true;

                MessageBox.Show($"✅ Стол #{table.TableNumber} свободен!\nДата: {selectedDate:dd.MM.yyyy}\nВремя: {selectedTime:hh\\:mm}",
                    "Доступно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // ИСПРАВЛЕННЫЙ МЕТОД СОЗДАНИЯ БРОНИ
        // ============================================================
        private void BtnCreateReservation_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cmbTables.SelectedIndex < 0 || cmbTables.SelectedIndex >= _tables.Count)
                {
                    MessageBox.Show("Выберите стол!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var table = _tables[cmbTables.SelectedIndex];
                var selectedDate = dtpDate.Value.Date;
                var selectedTime = dtpTime.Value.TimeOfDay;
                int guests = (int)nudGuests.Value;

                // Проверка: прошедшая дата
                if (selectedDate < DateTime.Today)
                {
                    MessageBox.Show("❌ Нельзя бронировать на прошедшую дату!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка: нельзя бронировать на прошедшее время сегодня
                if (selectedDate == DateTime.Today && dtpTime.Value < DateTime.Now)
                {
                    MessageBox.Show("❌ Нельзя бронировать на прошедшее время!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка: рабочее время
                if (selectedTime.Hours < 9 || selectedTime.Hours >= 23)
                {
                    MessageBox.Show("❌ Ресторан работает с 9:00 до 23:00!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка: вместимость стола
                if (guests > table.Capacity)
                {
                    MessageBox.Show($"❌ Стол вмещает максимум {table.Capacity} человек!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка: есть ли уже бронь
                var existingReservation = _context.Reservations
                    .FirstOrDefault(r => r.TableID == table.TableID &&
                                         r.ReservationDate == selectedDate &&
                                         r.ReservationTime == selectedTime &&
                                         r.Status != "Отменена");

                if (existingReservation != null)
                {
                    MessageBox.Show($"❌ Стол уже занят!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var reservation = new Reservation
                {
                    TableID = table.TableID,
                    UserID = _currentUser.UserID,
                    ReservationDate = selectedDate,
                    ReservationTime = selectedTime,
                    EndTime = selectedTime.Add(TimeSpan.FromHours(2)),
                    GuestCount = guests,
                    Status = "Активна",
                    CreatedAt = DateTime.Now
                };

                _context.Reservations.Add(reservation);
                _context.SaveChanges();

                table.Status = "Забронирован";
                _context.SaveChanges();

                _currentOrder.ReservationID = reservation.ReservationID;
                _currentOrder.OrderStatus = "Составление";

                btnCreateReservation.Enabled = false;
                btnConfirmOrder.Enabled = true;
                btnCheckAvailability.Enabled = false;
                cmbTables.Enabled = false;
                dtpDate.Enabled = false;
                dtpTime.Enabled = false;
                nudGuests.Enabled = false;

                lblOrderInfo.Text = $"📋 Заказ к брони #{reservation.ReservationID} | Стол #{table.TableNumber} | {selectedDate:dd.MM.yyyy} {selectedTime:hh\\:mm}";
                lblOrderInfo.ForeColor = Color.FromArgb(44, 62, 80);

                MessageBox.Show($"✅ Бронь создана!\nСтол: #{table.TableNumber}\nДата: {selectedDate:dd.MM.yyyy}\nВремя: {selectedTime:hh\\:mm}\nГостей: {guests}\n\nТеперь можно добавлять блюда в заказ!",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // РАБОТА С ЗАКАЗОМ
        // ============================================================

        private void BtnAddDish_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentOrder.ReservationID == null)
                {
                    MessageBox.Show("Сначала создайте бронь!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_currentOrder.OrderStatus != "Составление")
                {
                    MessageBox.Show("Заказ уже оформлен, редактирование невозможно", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var menuItem = cmbDishes.SelectedItem as MenuItem;
                if (menuItem == null) return;

                int quantity = (int)nudQuantity.Value;

                if (quantity > 50)
                {
                    MessageBox.Show($"Недостаточно порций блюда \"{menuItem.Name}\". Доступно: 50 порций", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var existingItem = _currentOrder.OrderItems
                    .FirstOrDefault(x => x.MenuItemID == menuItem.MenuItemID);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    MessageBox.Show($"✅ Блюдо \"{menuItem.Name}\" уже есть в заказе!\nДобавлено ещё {quantity} порций.\nТеперь всего: {existingItem.Quantity} порций",
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var orderItem = new OrderItem
                    {
                        OrderItemID = _currentOrder.OrderItems.Count + 1,
                        MenuItemID = menuItem.MenuItemID,
                        Quantity = quantity,
                        UnitPrice = menuItem.Price,
                        Discount = 0,
                        MenuItem = menuItem
                    };

                    _currentOrder.OrderItems.Add(orderItem);
                    MessageBox.Show($"✅ Блюдо \"{menuItem.Name}\" в количестве {quantity} порций добавлено в заказ",
                        "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                RefreshOrderGrid();
                UpdateTotal();
                nudQuantity.Value = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemoveDish_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentOrder.OrderStatus != "Составление")
                {
                    MessageBox.Show("Заказ уже оформлен, редактирование невозможно", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvOrderItems.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите позицию для удаления", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int index = dgvOrderItems.SelectedRows[0].Index;
                var item = _currentOrder.OrderItems.ToList()[index];

                if (item.Quantity > 1)
                {
                    var result = MessageBox.Show($"Блюдо \"{item.MenuItem?.Name}\" ({item.Quantity} порций)\n\n" +
                        $"Нажмите 'Да' чтобы удалить ВСЁ\n" +
                        $"Нажмите 'Нет' чтобы уменьшить на 1 порцию",
                        "Удаление блюда",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _currentOrder.OrderItems.Remove(item);
                        MessageBox.Show($"✅ Блюдо полностью удалено из заказа",
                            "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (result == DialogResult.No)
                    {
                        item.Quantity -= 1;
                        MessageBox.Show($"✅ Количество блюда уменьшено на 1. Осталось: {item.Quantity} порций",
                            "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    var result = MessageBox.Show($"Удалить блюдо \"{item.MenuItem?.Name}\" из заказа?",
                        "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _currentOrder.OrderItems.Remove(item);
                        MessageBox.Show($"✅ Блюдо удалено из заказа",
                            "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        return;
                    }
                }

                RefreshOrderGrid();
                UpdateTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshOrderGrid()
        {
            dgvOrderItems.Rows.Clear();
            foreach (var item in _currentOrder.OrderItems)
            {
                dgvOrderItems.Rows.Add(
                    item.OrderItemID,
                    item.MenuItem?.Name ?? "Блюдо",
                    item.Quantity,
                    item.UnitPrice,
                    item.Discount,
                    item.Quantity * item.UnitPrice * (1 - item.Discount / 100)
                );
            }
        }

        private void UpdateTotal()
        {
            decimal total = 0;
            foreach (var item in _currentOrder.OrderItems)
            {
                total += item.Quantity * item.UnitPrice * (1 - item.Discount / 100);
            }
            _currentOrder.TotalSum = total;
            lblTotal.Text = $"{total:F2} ₽";
        }

        private void UpdateOrderInfo()
        {
            lblOrderInfo.Text = $"📋 Заказ: {_currentOrder.OrderStatus}";
            if (_currentOrder.ReservationID != null)
            {
                lblOrderInfo.Text += $" | Бронь #{_currentOrder.ReservationID}";
            }
        }

        // ============================================================
        // ОФОРМЛЕНИЕ ЗАКАЗА
        // ============================================================
        private async void BtnConfirmOrder_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentOrder.ReservationID == null)
                {
                    MessageBox.Show("Сначала создайте бронь!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_currentOrder.OrderItems.Count == 0)
                {
                    MessageBox.Show("Нельзя оформить пустой заказ", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var reservation = _context.Reservations
                    .Include(r => r.Table)
                    .FirstOrDefault(r => r.ReservationID == _currentOrder.ReservationID && r.Status != "Отменена");

                if (reservation == null)
                {
                    MessageBox.Show("❌ Бронь не найдена или отменена!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int tableNumber = reservation.Table?.TableNumber ?? 0;

                string orderDetails = "📋 СОСТАВ ЗАКАЗА:\n\n";
                foreach (var item in _currentOrder.OrderItems)
                {
                    decimal sum = item.Quantity * item.UnitPrice * (1 - item.Discount / 100);
                    var menuItem = _context.MenuItems.Find(item.MenuItemID);
                    orderDetails += $"• {menuItem?.Name ?? "Блюдо"} x{item.Quantity} = {sum:F2} ₽\n";
                }
                orderDetails += $"\n━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
                orderDetails += $"ИТОГО: {_currentOrder.TotalSum:F2} ₽";

                var result = MessageBox.Show($"{orderDetails}\n\nОформить заказ?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    using var transaction = _context.Database.BeginTransaction();

                    try
                    {
                        string insertOrderSql = @"
                            INSERT INTO Orders (ReservationID, ShiftID, OrderStatus, OrderDate, TotalSum, TableNumber)
                            VALUES (@ReservationID, @ShiftID, @OrderStatus, @OrderDate, @TotalSum, @TableNumber);
                            SELECT SCOPE_IDENTITY();";

                        var parameters = new[]
                        {
                            new SqlParameter("@ReservationID", _currentOrder.ReservationID),
                            new SqlParameter("@ShiftID", 1),
                            new SqlParameter("@OrderStatus", "Оформлен"),
                            new SqlParameter("@OrderDate", DateTime.Now),
                            new SqlParameter("@TotalSum", _currentOrder.TotalSum),
                            new SqlParameter("@TableNumber", tableNumber)
                        };

                        var orderIdObj = await _context.Database.ExecuteSqlRawAsync(insertOrderSql, parameters);
                        int orderId = Convert.ToInt32(orderIdObj);

                        foreach (var item in _currentOrder.OrderItems)
                        {
                            string insertItemSql = @"
                                INSERT INTO OrderItems (OrderID, MenuItemID, Quantity, UnitPrice, Discount)
                                VALUES (@OrderID, @MenuItemID, @Quantity, @UnitPrice, @Discount)";

                            var itemParams = new[]
                            {
                                new SqlParameter("@OrderID", orderId),
                                new SqlParameter("@MenuItemID", item.MenuItemID),
                                new SqlParameter("@Quantity", item.Quantity),
                                new SqlParameter("@UnitPrice", item.UnitPrice),
                                new SqlParameter("@Discount", item.Discount)
                            };

                            await _context.Database.ExecuteSqlRawAsync(insertItemSql, itemParams);
                        }

                        transaction.Commit();

                        _currentOrder.OrderStatus = "Оформлен";
                        _currentOrder.OrderID = orderId;
                        lblStatus.Text = "Статус: Оформлен ✅";
                        lblStatus.ForeColor = Color.Green;
                        lblTotal.Text = $"✅ ЗАКАЗ #{orderId} ОФОРМЛЕН!";
                        lblOrderInfo.Text = $"📋 Заказ #{orderId} | Стол #{tableNumber} | {DateTime.Now:dd.MM.yyyy HH:mm}";

                        MessageBox.Show($"✅ Заказ №{orderId} оформлен на сумму {_currentOrder.TotalSum:F2} ₽\n\n" +
                            $"Стол: #{tableNumber}\n" +
                            $"Количество блюд: {_currentOrder.OrderItems.Count} позиций",
                            "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        btnConfirmOrder.Enabled = false;
                        btnAddDish.Enabled = false;
                        btnRemoveDish.Enabled = false;
                        cmbDishes.Enabled = false;
                        nudQuantity.Enabled = false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        string errorMessage = ex.Message;
                        if (ex.InnerException != null)
                        {
                            errorMessage += $"\n\nВнутренняя ошибка: {ex.InnerException.Message}";
                        }

                        MessageBox.Show($"❌ Ошибка при сохранении заказа:\n\n{errorMessage}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}