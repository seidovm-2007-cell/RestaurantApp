using RestaurantApp.Core.Models;
using RestaurantApp.Services;
using RestaurantApp.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.UI.Forms
{
    public partial class ReservationForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly User _currentUser;
        private readonly OrderService _orderService;
        private readonly AppDbContext _context;
        private List<Table> _tables = new();
        private int _selectedReservationId = 0;

        private ComboBox cmbTable = new();
        private DateTimePicker dtpDate = new();
        private DateTimePicker dtpTime = new();
        private NumericUpDown nudGuests = new();
        private Button btnCreate = new();
        private Button btnCancel = new();
        private DataGridView dgvReservations = new();
        private Label lblReservationInfo = new();
        private Label lblNoReservations = new(); // Добавляем для отображения "Нет броней"

        public ReservationForm(IServiceProvider serviceProvider, User currentUser)
        {
            _serviceProvider = serviceProvider;
            _currentUser = currentUser;
            _orderService = (OrderService)serviceProvider.GetService(typeof(OrderService))!;
            _context = new AppDbContext();
            InitializeComponent();
            LoadTables();
            LoadReservations();
            SetupDateRestrictions();
        }

        private void InitializeComponent()
        {
            this.Text = "📅 Бронирования - Ресторан";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            int y = 20;

            var lblTitle = new Label
            {
                Text = "📅 Мои бронирования",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);
            y += 50;

            var groupBox = new GroupBox
            {
                Text = "Забронировать столик",
                Location = new Point(20, y),
                Size = new Size(450, 250),
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
            cmbTable.SelectedIndexChanged += CmbTable_SelectedIndexChanged!;
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
                MinDate = DateTime.Today,
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
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(dtpTime);
            gy += 35;

            // Гости
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
                Maximum = 10,
                Value = 2,
                Font = new Font("Segoe UI", 10)
            };
            groupBox.Controls.Add(nudGuests);
            gy += 35;

            // Информация
            lblReservationInfo = new Label
            {
                Text = "📌 Выберите стол, дату и время",
                Location = new Point(20, gy),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            groupBox.Controls.Add(lblReservationInfo);
            gy += 35;

            // Кнопка
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
            y += 270;

            // СПИСОК БРОНЕЙ
            var lblReservations = new Label
            {
                Text = "📋 Мои бронирования:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(20, y),
                AutoSize = true
            };
            this.Controls.Add(lblReservations);
            y += 30;

            // Label для сообщения "Нет броней"
            lblNoReservations = new Label
            {
                Text = "❌ У вас пока нет броней",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(20, y + 100),
                AutoSize = true,
                Visible = false
            };
            this.Controls.Add(lblNoReservations);

            dgvReservations = new DataGridView
            {
                Location = new Point(20, y),
                Size = new Size(1050, 250),
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
            dgvReservations.Columns.Add("Date", "Дата");
            dgvReservations.Columns.Add("Time", "Время");
            dgvReservations.Columns.Add("Guests", "Гостей");
            dgvReservations.Columns.Add("Status", "Статус");

            dgvReservations.SelectionChanged += DgvReservations_SelectionChanged!;
            this.Controls.Add(dgvReservations);
            y += 270;

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
        }

        private void CmbTable_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbTable.SelectedIndex >= 0 && cmbTable.SelectedIndex < _tables.Count)
            {
                var table = _tables[cmbTable.SelectedIndex];
                nudGuests.Maximum = table.Capacity;
                lblReservationInfo.Text = $"📌 Стол #{table.TableNumber}, вместимость: {table.Capacity} чел.";
            }
        }

        private void LoadTables()
        {
            try
            {
                _tables = _context.Tables.ToList();
                cmbTable.Items.Clear();
                foreach (var table in _tables)
                {
                    cmbTable.Items.Add($"Стол #{table.TableNumber} ({table.Capacity} чел.)");
                }
                if (cmbTable.Items.Count > 0)
                    cmbTable.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки столов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Заглушка, если не удалось загрузить из БД
                for (int i = 1; i <= 10; i++)
                {
                    cmbTable.Items.Add($"Стол #{i} ({(i <= 8 ? 4 : 2)} чел.)");
                }
                cmbTable.SelectedIndex = 0;
            }
        }

        private void LoadReservations()
        {
            try
            {
                dgvReservations.Rows.Clear();
                lblNoReservations.Visible = false;

                var reservations = _context.Reservations
                    .Where(r => r.UserID == _currentUser.UserID)
                    .Include(r => r.Table)
                    .OrderByDescending(r => r.ReservationDate)
                    .ToList();

                if (reservations.Count == 0)
                {
                    // Показываем сообщение, что нет броней
                    lblNoReservations.Visible = true;
                    dgvReservations.Visible = true; // Оставляем видимым, но без строк
                    return;
                }

                foreach (var r in reservations)
                {
                    dgvReservations.Rows.Add(
                        r.ReservationID.ToString(),
                        $"#{r.Table?.TableNumber ?? 0}",
                        r.ReservationDate.ToString("dd.MM.yyyy"),
                        r.ReservationTime.ToString(@"hh\:mm"),
                        r.GuestCount.ToString(),
                        r.Status ?? "Активна"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки броней: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDateRestrictions()
        {
            dtpDate.MinDate = DateTime.Today;
            dtpDate.MaxDate = DateTime.Today.AddDays(30);
        }

        private void DgvReservations_SelectionChanged(object? sender, EventArgs e)
        {
            try
            {
                if (dgvReservations.SelectedRows.Count > 0)
                {
                    var row = dgvReservations.SelectedRows[0];
                    if (row.Cells["Id"].Value != null &&
                        !string.IsNullOrEmpty(row.Cells["Id"].Value.ToString()))
                    {
                        if (int.TryParse(row.Cells["Id"].Value.ToString(), out int id))
                        {
                            _selectedReservationId = id;
                        }
                        else
                        {
                            _selectedReservationId = 0;
                        }
                    }
                    else
                    {
                        _selectedReservationId = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не показываем пользователю
                Console.WriteLine($"Ошибка в SelectionChanged: {ex.Message}");
            }
        }

        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            try
            {
                var selectedDate = dtpDate.Value.Date;
                if (selectedDate < DateTime.Today)
                {
                    MessageBox.Show("❌ Нельзя бронировать на прошедшую дату!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int tableIndex = cmbTable.SelectedIndex;
                if (tableIndex < 0 || tableIndex >= _tables.Count)
                {
                    MessageBox.Show("Выберите столик!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var table = _tables[tableIndex];
                int guests = (int)nudGuests.Value;

                if (guests > table.Capacity)
                {
                    MessageBox.Show($"❌ Стол вмещает максимум {table.Capacity} человек!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Проверка на конфликт
                var conflict = _context.Reservations
                    .Any(r => r.TableID == table.TableID &&
                              r.ReservationDate == selectedDate &&
                              r.ReservationTime == dtpTime.Value.TimeOfDay &&
                              r.Status != "Отменена");

                if (conflict)
                {
                    MessageBox.Show($"❌ Стол уже занят в это время!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var reservation = new Reservation
                {
                    TableID = table.TableID,
                    UserID = _currentUser.UserID,
                    ReservationDate = selectedDate,
                    ReservationTime = dtpTime.Value.TimeOfDay,
                    EndTime = dtpTime.Value.TimeOfDay.Add(TimeSpan.FromHours(2)),
                    GuestCount = guests,
                    Status = "Активна",
                    CreatedAt = DateTime.Now
                };

                _context.Reservations.Add(reservation);
                _context.SaveChanges();

                LoadReservations();

                MessageBox.Show($"✅ Бронь создана!\n" +
                    $"Стол: #{table.TableNumber}\n" +
                    $"Дата: {selectedDate:dd.MM.yyyy}\n" +
                    $"Время: {dtpTime.Value:HH:mm}\n" +
                    $"Гостей: {guests}",
                    "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

                var row = dgvReservations.SelectedRows[0];

                // Проверяем, что ID не пустой
                if (row.Cells["Id"].Value == null ||
                    string.IsNullOrEmpty(row.Cells["Id"].Value.ToString()))
                {
                    MessageBox.Show("Выберите существующую бронь!", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!int.TryParse(row.Cells["Id"].Value.ToString(), out int reservationId))
                {
                    MessageBox.Show("Ошибка идентификации брони!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Отменить выбранную бронь?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var reservation = _context.Reservations.Find(reservationId);
                    if (reservation != null)
                    {
                        // Проверяем, что бронь еще активна
                        if (reservation.Status == "Отменена")
                        {
                            MessageBox.Show("Эта бронь уже отменена!", "Информация",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        reservation.Status = "Отменена";
                        _context.SaveChanges();

                        LoadReservations();
                        MessageBox.Show("✅ Бронь отменена!", "Успешно",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Бронь не найдена!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Ошибка при отмене брони: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}