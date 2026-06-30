using RestaurantApp.Core.Models;
using RestaurantApp.Services;

namespace RestaurantApp.UI.Forms
{
    public partial class OrdersForm : Form
    {
        private readonly OrderService _orderService;
        private readonly User _currentUser;

        public OrdersForm(OrderService orderService, User currentUser)
        {
            _orderService = orderService;
            _currentUser = currentUser;
            InitializeComponent();
            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                var orders = await _orderService.GetActiveOrdersAsync();
                // TODO: Отобразить заказы в DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // OrdersForm
            // 
            ClientSize = new Size(1080, 714);
            Name = "OrdersForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Заказы - Ресторан";
            ResumeLayout(false);
        }
    }
}