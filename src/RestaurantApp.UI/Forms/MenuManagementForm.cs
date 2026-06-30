namespace RestaurantApp.UI.Forms
{
    public partial class MenuManagementForm : Form
    {
        private DataGridView dgvMenu = new();
        private TextBox txtName = new();
        private TextBox txtCategory = new();
        private NumericUpDown nudPrice = new();
        private NumericUpDown nudStock = new();
        private Button btnAdd = new();
        private Button btnEdit = new();
        private Button btnDelete = new();

        public MenuManagementForm()
        {
            InitializeComponent();
            LoadMenu();
        }

        private void InitializeComponent()
        {
            this.Text = "Управление меню - Ресторан";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            int y = 20;

            // Заголовок
            var lblTitle = new Label
            {
                Text = "🍕 Управление меню",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            y += 50;

            // DataGridView
            dgvMenu = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(550, 450),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowHeadersVisible = false
            };

            dgvMenu.Columns.Add("Id", "ID");
            dgvMenu.Columns.Add("Name", "Название");
            dgvMenu.Columns.Add("Category", "Категория");
            dgvMenu.Columns.Add("Price", "Цена");
            dgvMenu.Columns.Add("Stock", "На складе");

            dgvMenu.Columns["Id"].Width = 40;
            dgvMenu.Columns["Name"].Width = 150;
            dgvMenu.Columns["Category"].Width = 100;
            dgvMenu.Columns["Price"].Width = 80;
            dgvMenu.Columns["Stock"].Width = 80;

            dgvMenu.SelectionChanged += DgvMenu_SelectionChanged!;

            // Панель редактирования - создаем элементы вручную
            var groupBox = new GroupBox
            {
                Text = "Редактирование блюда",
                Location = new Point(600, y),
                Size = new Size(270, 300),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            // Поле "Название"
            var lblName = new Label
            {
                Text = "Название:",
                Location = new Point(10, 30),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(lblName);

            txtName = new TextBox
            {
                Location = new Point(100, 30),
                Size = new Size(150, 25)
            };
            groupBox.Controls.Add(txtName);

            // Поле "Категория"
            var lblCategory = new Label
            {
                Text = "Категория:",
                Location = new Point(10, 65),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(lblCategory);

            txtCategory = new TextBox
            {
                Location = new Point(100, 65),
                Size = new Size(150, 25)
            };
            groupBox.Controls.Add(txtCategory);

            // Поле "Цена"
            var lblPrice = new Label
            {
                Text = "Цена:",
                Location = new Point(10, 100),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(lblPrice);

            nudPrice = new NumericUpDown
            {
                Location = new Point(100, 100),
                Size = new Size(150, 25),
                DecimalPlaces = 2,
                Minimum = 0,
                Maximum = 9999,
                Value = 0
            };
            groupBox.Controls.Add(nudPrice);

            // Поле "На складе"
            var lblStock = new Label
            {
                Text = "На складе:",
                Location = new Point(10, 135),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(lblStock);

            nudStock = new NumericUpDown
            {
                Location = new Point(100, 135),
                Size = new Size(150, 25),
                Minimum = 0,
                Maximum = 9999,
                Value = 0
            };
            groupBox.Controls.Add(nudStock);

            // Кнопки
            btnAdd = new Button
            {
                Text = "➕ Добавить",
                Location = new Point(20, 180),
                Size = new Size(230, 35),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnAdd.Click += BtnAdd_Click!;
            groupBox.Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "✏️ Редактировать",
                Location = new Point(20, 225),
                Size = new Size(230, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnEdit.Click += BtnEdit_Click!;
            groupBox.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "🗑️ Удалить",
                Location = new Point(20, 270),
                Size = new Size(230, 35),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnDelete.Click += BtnDelete_Click!;
            groupBox.Controls.Add(btnDelete);

            this.Controls.Add(lblTitle);
            this.Controls.Add(dgvMenu);
            this.Controls.Add(groupBox);
        }

        private void LoadMenu()
        {
            dgvMenu.Rows.Clear();
            var menuItems = new (int id, string name, string category, decimal price, int stock)[]
            {
                (1, "Борщ", "Супы", 250, 50),
                (2, "Стейк", "Горячие блюда", 950, 30),
                (3, "Цезарь", "Салаты", 320, 60),
                (4, "Чай", "Напитки", 80, 200),
                (5, "Тирамису", "Десерты", 290, 30)
            };

            foreach (var item in menuItems)
            {
                dgvMenu.Rows.Add(item.id, item.name, item.category, item.price, item.stock);
            }
        }

        private void DgvMenu_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvMenu.SelectedRows.Count > 0)
            {
                var row = dgvMenu.SelectedRows[0];
                txtName.Text = row.Cells["Name"].Value?.ToString() ?? "";
                txtCategory.Text = row.Cells["Category"].Value?.ToString() ?? "";
                nudPrice.Value = Convert.ToDecimal(row.Cells["Price"].Value ?? 0);
                nudStock.Value = Convert.ToDecimal(row.Cells["Stock"].Value ?? 0);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                var name = txtName.Text.Trim();
                var category = txtCategory.Text.Trim();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(category))
                {
                    MessageBox.Show("Заполните все поля", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dgvMenu.Rows.Add(
                    dgvMenu.Rows.Count + 1,
                    name,
                    category,
                    nudPrice.Value,
                    nudStock.Value
                );

                MessageBox.Show($"Блюдо \"{name}\" добавлено!", "Успешно",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvMenu.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите блюдо для редактирования", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var row = dgvMenu.SelectedRows[0];
                row.Cells["Name"].Value = txtName.Text.Trim();
                row.Cells["Category"].Value = txtCategory.Text.Trim();
                row.Cells["Price"].Value = nudPrice.Value;
                row.Cells["Stock"].Value = nudStock.Value;

                MessageBox.Show("Блюдо обновлено!", "Успешно",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            try
            {
                if (dgvMenu.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите блюдо для удаления", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var name = dgvMenu.SelectedRows[0].Cells["Name"].Value?.ToString();

                var result = MessageBox.Show($"Удалить блюдо \"{name}\"?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgvMenu.Rows.RemoveAt(dgvMenu.SelectedRows[0].Index);
                    MessageBox.Show("Блюдо удалено!", "Успешно",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtCategory.Text = "";
            nudPrice.Value = 0;
            nudStock.Value = 0;
        }
    }
}