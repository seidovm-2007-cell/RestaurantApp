using RestaurantApp.Core.Models;
using RestaurantApp.Services;

namespace RestaurantApp.UI.Forms
{
    public partial class OrderForm : Form
    {
        private readonly OrderService _orderService;
        private readonly User _currentUser;
        private Order _currentOrder;
        private List<MenuItem> _menuItems;
        private ComboBox cmbTables = new();
        private DataGridView dgvOrderItems = new();
        private ComboBox cmbDishes = new();
        private NumericUpDown nudQuantity = new();
        private Button btnAddDish = new();
        private Button btnRemoveDish = new();
        private Button btnConfirmOrder = new();
        private Label lblTotal = new();
        private Label lblStatus = new();

        public OrderForm(OrderService orderService, User currentUser)
        {
            _orderService = orderService;
            _currentUser = currentUser;
            _menuItems = new List<MenuItem>();
            InitializeComponent();
            LoadMenuItems();
            LoadTables();
        }

        private void InitializeComponent()
        {
            this.Text = "Создание заказа - Ресторан";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "📋 Создание нового заказа",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Выбор стола
            var lblTable = new Label
            {
                Text = "Стол:",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 70),
                AutoSize = true
            };

            cmbTables = new ComboBox
            {
                Location = new Point(120, 68),
                Size = new Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };

            // Информация о статусе
            lblStatus = new Label
            {
                Text = "Статус: Составление",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Orange,
                Location = new Point(20, 110),
                AutoSize = true
            };

            // DataGridView для позиций заказа
            dgvOrderItems = new DataGridView
            {
                Location = new Point(20, 150),
                Size = new Size(550, 300),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false
            };

            // Настройка колонок
            dgvOrderItems.Columns.Add("Id", "ID");
            dgvOrderItems.Columns.Add("Name", "Блюдо");
            dgvOrderItems.Columns.Add("Qty", "Кол-во");
            dgvOrderItems.Columns.Add("Price", "Цена");
            dgvOrderItems.Columns.Add("Discount", "Скидка %");
            dgvOrderItems.Columns.Add("Total", "Сумма");
            dgvOrderItems.Columns["Id"].Width = 40;
            dgvOrderItems.Columns["Name"].Width = 150;
            dgvOrderItems.Columns["Qty"].Width = 60;
            dgvOrderItems.Columns["Price"].Width = 70;
            dgvOrderItems.Columns["Discount"].Width = 70;
            dgvOrderItems.Columns["Total"].Width = 90;

            // Панель добавления блюда
            var lblAddDish = new Label
            {
                Text = "Добавить блюдо:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(600, 150),
                AutoSize = true
            };

            var lblDish = new Label
            {
                Text = "Блюдо:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(600, 180),
                AutoSize = true
            };

            cmbDishes = new ComboBox
            {
                Location = new Point(600, 200),
                Size = new Size(250, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };

            var lblQty = new Label
            {
                Text = "Количество:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(600, 240),
                AutoSize = true
            };

            nudQuantity = new NumericUpDown
            {
                Location = new Point(600, 260),
                Size = new Size(80, 30),
                Minimum = 1,
                Maximum = 99,
                Value = 1,
                Font = new Font("Segoe UI", 11)
            };

            btnAddDish = new Button
            {
                Text = "➕ Добавить",
                Location = new Point(600, 300),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnAddDish.Click += BtnAddDish_Click!;

            btnRemoveDish = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(600, 350),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnRemoveDish.Click += BtnRemoveDish_Click!;

            // Итоговая сумма
            var lblTotalLabel = new Label
            {
                Text = "ИТОГО:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, 470),
                AutoSize = true
            };

            lblTotal = new Label
            {
                Text = "0.00 ₽",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(150, 465),
                AutoSize = true
            };

            // Кнопки управления
            btnConfirmOrder = new Button
            {
                Text = "✅ Оформить заказ",
                Location = new Point(600, 420),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnConfirmOrder.Click += BtnConfirmOrder_Click!;

            var btnClose = new Button
            {
                Text = "✖ Закрыть",
                Location = new Point(700, 500),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => this.Close();

            // Добавление элементов
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblTable);
            this.Controls.Add(cmbTables);
            this.Controls.Add(lblStatus);
            this.Controls.Add(dgvOrderItems);
            this.Controls.Add(lblAddDish);
            this.Controls.Add(lblDish);
            this.Controls.Add(cmbDishes);
            this.Controls.Add(lblQty);
            this.Controls.Add(nudQuantity);
            this.Controls.Add(btnAddDish);
            this.Controls.Add(btnRemoveDish);
            this.Controls.Add(lblTotalLabel);
            this.Controls.Add(lblTotal);
            this.Controls.Add(btnConfirmOrder);
            this.Controls.Add(btnClose);
        }

        private async void LoadMenuItems()
        {
            try
            {
                _menuItems = new List<MenuItem>
                {
                    new MenuItem { MenuItemID = 1, Name = "Борщ", Price = 250, Category = "Супы" },
                    new MenuItem { MenuItemID = 2, Name = "Стейк", Price = 950, Category = "Горячие блюда" },
                    new MenuItem { MenuItemID = 3, Name = "Цезарь", Price = 320, Category = "Салаты" },
                    new MenuItem { MenuItemID = 4, Name = "Чай", Price = 80, Category = "Напитки" },
                    new MenuItem { MenuItemID = 5, Name = "Тирамису", Price = 290, Category = "Десерты" }
                };

                cmbDishes.DataSource = _menuItems;
                cmbDishes.DisplayMember = "Name";
                cmbDishes.ValueMember = "MenuItemID";

                _currentOrder = new Order
                {
                    OrderID = new Random().Next(100, 999),
                    OrderStatus = "Составление",
                    OrderDate = DateTime.Now,
                    TotalSum = 0,
                    OrderItems = new List<OrderItem>()
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки меню: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTables()
        {
            var tables = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            cmbTables.DataSource = tables;
            cmbTables.SelectedIndex = 0;
        }

        private void BtnAddDish_Click(object? sender, EventArgs e)
        {
            try
            {
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
                RefreshOrderGrid();
                UpdateTotal();

                MessageBox.Show($"Блюдо \"{menuItem.Name}\" в количестве {quantity} порций добавлено в заказ",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (dgvOrderItems.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите позицию для удаления", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int index = dgvOrderItems.SelectedRows[0].Index;

                // Используем ToList() для доступа к элементу по индексу
                var item = _currentOrder.OrderItems.ToList()[index];

                var result = MessageBox.Show($"Удалить блюдо \"{item.MenuItem?.Name}\" из заказа?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Используем ToList() и удаляем по индексу
                    var itemsList = _currentOrder.OrderItems.ToList();
                    itemsList.RemoveAt(index);
                    _currentOrder.OrderItems = itemsList;

                    RefreshOrderGrid();
                    UpdateTotal();

                    MessageBox.Show($"Блюдо удалено из заказа", "Успешно",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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

        private async void BtnConfirmOrder_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentOrder.OrderItems.Count == 0)
                {
                    MessageBox.Show("Нельзя оформить пустой заказ", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show($"Оформить заказ на сумму {_currentOrder.TotalSum:F2} ₽?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _currentOrder.OrderStatus = "Оформлен";
                    lblStatus.Text = "Статус: Оформлен ✅";
                    lblStatus.ForeColor = Color.Green;
                    lblTotal.Text = "✅ ЗАКАЗ ОФОРМЛЕН!";

                    MessageBox.Show($"Заказ №{_currentOrder.OrderID} оформлен на сумму {_currentOrder.TotalSum:F2} ₽",
                        "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnConfirmOrder.Enabled = false;
                    btnAddDish.Enabled = false;
                    btnRemoveDish.Enabled = false;
                    cmbDishes.Enabled = false;
                    nudQuantity.Enabled = false;
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