using System;
using System.Threading.Tasks;
using RestaurantApp.Core.Models;
using RestaurantApp.Data.Repositories;

namespace RestaurantApp.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // ВРЕМЕННО: ВХОД БЕЗ ПРОВЕРКИ ПАРОЛЯ
        public async Task<User?> LoginAsync(string login, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login))
                {
                    return null;
                }

                // Ищем пользователя только по логину (пароль не проверяем!)
                return await _userRepository.GetByLoginAsync(login);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<User> RegisterAsync(string login, string password, string lastName, string firstName, string? middleName)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Логин не может быть пустым");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым");

            if (password.Length < 6)
                throw new ArgumentException("Пароль должен быть не менее 6 символов");

            if (await _userRepository.LoginExistsAsync(login))
                throw new InvalidOperationException($"Пользователь с логином '{login}' уже существует");

            var user = new User
            {
                Login = login,
                PasswordHash = "temp",
                LastName = lastName,
                FirstName = firstName,
                MiddleName = middleName,
                RoleID = 8,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            return await _userRepository.AddAsync(user);
        }

        public async Task<bool> IsLoginExistsAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return false;

            return await _userRepository.LoginExistsAsync(login);
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return null;

            return await _userRepository.GetByLoginAsync(login);
        }

        public async Task<bool> CheckUserExistsAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return false;

            return await _userRepository.LoginExistsAsync(login);
        }
    }
}