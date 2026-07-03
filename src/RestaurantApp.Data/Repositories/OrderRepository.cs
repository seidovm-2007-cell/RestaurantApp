using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Models;
using RestaurantApp.Data.Context;

namespace RestaurantApp.Data.Repositories
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                // УБИРАЕМ: .Include(o => o.Reservation)
                .Include(o => o.Shift)
                .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.OrderStatus != "Отменен" && o.OrderStatus != "Готов")
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByShiftAsync(int shiftId)
        {
            return await _dbSet
                .Where(o => o.ShiftID == shiftId)
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSumByOrderAsync(int orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderID == orderId)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice * (1 - oi.Discount / 100));
        }
    }
}