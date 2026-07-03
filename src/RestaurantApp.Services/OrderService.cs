using RestaurantApp.Core.Models;
using RestaurantApp.Data.Context;
using RestaurantApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace RestaurantApp.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly Repository<Order> _orderRepository;
        private readonly Repository<OrderItem> _orderItemRepository;
        private readonly Repository<MenuItem> _menuItemRepository;

        public OrderService(AppDbContext context)
        {
            _context = context;
            _orderRepository = new Repository<Order>(context);
            _orderItemRepository = new Repository<OrderItem>(context);
            _menuItemRepository = new Repository<MenuItem>(context);
        }

        public async Task<Order> CreateOrderAsync(int shiftId, int? reservationId = null)
        {
            var order = new Order
            {
                ShiftID = shiftId,
                ReservationID = reservationId,
                OrderStatus = "Составление",
                OrderDate = DateTime.Now,
                TotalSum = 0
            };

            return await _orderRepository.AddAsync(order);
        }

        public async Task<decimal> CalculateTotalAsync(int orderId)
        {
            var result = await _context.OrderItems
                .Where(oi => oi.OrderID == orderId)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice * (1 - oi.Discount / 100));

            return result;
        }

        public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.OrderStatus != "Отменен" && o.OrderStatus != "Готов")
                .OrderBy(o => o.OrderDate)
                .ToListAsync();

            return orders ?? new List<Order>();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        public async Task<Order> AddDishToOrderAsync(int orderId, int menuItemId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    throw new InvalidOperationException("Заказ не найден");

                if (order.OrderStatus != "Составление")
                    throw new InvalidOperationException("Заказ не может быть изменен");

                var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId);
                if (menuItem == null)
                    throw new InvalidOperationException("Блюдо не найдено");

                if (menuItem.StockQuantity < quantity)
                    throw new InvalidOperationException($"Недостаточно порций. Доступно: {menuItem.StockQuantity}");

                var orderItem = new OrderItem
                {
                    OrderID = orderId,
                    MenuItemID = menuItemId,
                    Quantity = quantity,
                    UnitPrice = menuItem.Price,
                    Discount = 0
                };

                await _orderItemRepository.AddAsync(orderItem);

                menuItem.StockQuantity -= quantity;
                await _menuItemRepository.UpdateAsync(menuItem);

                order.TotalSum = await CalculateTotalAsync(orderId);
                await _orderRepository.UpdateAsync(order);

                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order> ConfirmOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException("Заказ не найден");

            if (order.OrderStatus != "Составление")
                throw new InvalidOperationException("Заказ не может быть оформлен");

            var itemCount = await _context.OrderItems
                .Where(oi => oi.OrderID == orderId)
                .SumAsync(oi => oi.Quantity);

            if (itemCount == 0)
                throw new InvalidOperationException("Нельзя оформить пустой заказ");

            order.OrderStatus = "Оформлен";
            await _orderRepository.UpdateAsync(order);

            return order;
        }

        public async Task<Order> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException("Заказ не найден");

            var validTransitions = new Dictionary<string, string[]>
            {
                { "Оформлен", new[] { "Принят", "Отменен" } },
                { "Принят", new[] { "Готовится", "Отменен" } },
                { "Готовится", new[] { "Готов", "Отменен" } },
                { "Готов", new[] { "Принят на Выдачу" } }
            };

            if (!validTransitions.TryGetValue(order.OrderStatus, out var transitions) ||
                !transitions.Contains(newStatus))
            {
                throw new InvalidOperationException($"Недопустимый переход статуса с '{order.OrderStatus}' на '{newStatus}'");
            }

            order.OrderStatus = newStatus;
            await _orderRepository.UpdateAsync(order);

            return order;
        }
    }
}