using RestaurantApp.Core.Models;

namespace RestaurantApp.Core.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string login, string password);
        Task<User> RegisterAsync(string login, string password, string lastName, string firstName, string? middleName);
        Task<bool> IsLoginExistsAsync(string login);
        Task<User?> GetUserByIdAsync(int userId);
        string HashPassword(string password);
    }
}