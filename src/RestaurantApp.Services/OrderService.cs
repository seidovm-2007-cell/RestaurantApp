using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Models;
using RestaurantApp.Data.Context;
using RestaurantApp.Data.Repositories;

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

        public async Task<Order> AddDishToOrderAsync(int orderId, int menuItemId, int quantity)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null || order.OrderStatus != "Составление")
                    throw new InvalidOperationException("Заказ не может быть изменен");

                var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId);
                if (menuItem == null)
                    throw new InvalidOperationException("Блюдо не найдено");

                if (menuItem.StockQuantity < quantity)
                    throw new InvalidOperationException($"Недостаточно порций. Доступно: {menuItem.StockQuantity}");

                var promotion = await _context.Promotions
                    .FirstOrDefaultAsync(p => p.IsActive &&
                        (p.MenuItemID == menuItemId || p.Category == menuItem.Category));

                var discount = promotion?.DiscountPercent ?? 0;

                var orderItem = new OrderItem
                {
                    OrderID = orderId,
                    MenuItemID = menuItemId,
                    Quantity = quantity,
                    UnitPrice = menuItem.Price,
                    Discount = discount
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

        public async Task<Order> RemoveDishFromOrderAsync(int orderItemId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var orderItem = await _orderItemRepository.GetByIdAsync(orderItemId);
                if (orderItem == null)
                    throw new InvalidOperationException("Позиция не найдена");

                var order = await _orderRepository.GetByIdAsync(orderItem.OrderID);
                if (order == null || order.OrderStatus != "Составление")
                    throw new InvalidOperationException("Заказ не может быть изменен");

                var menuItem = await _menuItemRepository.GetByIdAsync(orderItem.MenuItemID);
                if (menuItem != null)
                {
                    menuItem.StockQuantity += orderItem.Quantity;
                    await _menuItemRepository.UpdateAsync(menuItem);
                }

                await _orderItemRepository.DeleteAsync(orderItemId);

                order.TotalSum = await CalculateTotalAsync(order.OrderID);
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
            if (order == null || order.OrderStatus != "Составление")
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

            if (!validTransitions.ContainsKey(order.OrderStatus) ||
                !validTransitions[order.OrderStatus].Contains(newStatus))
            {
                throw new InvalidOperationException($"Недопустимый переход статуса с '{order.OrderStatus}' на '{newStatus}'");
            }

            order.OrderStatus = newStatus;
            await _orderRepository.UpdateAsync(order);

            return order;
        }

        public async Task<decimal> CalculateTotalAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderID == orderId)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice * (1 - oi.Discount / 100));
        }

        public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.OrderStatus != "Отменен" && o.OrderStatus != "Готов")
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }
    }
}