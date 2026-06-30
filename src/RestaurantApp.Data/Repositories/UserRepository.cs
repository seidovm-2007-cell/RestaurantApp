using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Models;
using RestaurantApp.Data.Context;

namespace RestaurantApp.Data.Repositories  // ← ПРОВЕРЬТЕ!
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            if (string.IsNullOrEmpty(login))
                return null;

            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User?> GetByLoginAndPasswordAsync(string login, string passwordHash)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(passwordHash))
                return null;

            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login && u.PasswordHash == passwordHash);
        }

        public async Task<bool> LoginExistsAsync(string login)
        {
            if (string.IsNullOrEmpty(login))
                return false;

            return await _dbSet.AnyAsync(u => u.Login == login);
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive)
                .Include(u => u.Role)
                .OrderBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.PasswordHash = newPasswordHash;
            await UpdateAsync(user);
            return true;
        }
    }
}