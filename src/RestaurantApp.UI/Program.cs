using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestaurantApp.Data.Context;
using RestaurantApp.Data.Repositories;
using RestaurantApp.Services;
using RestaurantApp.UI.Forms;

namespace RestaurantApp.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                // Проверка подключения к БД
                if (!context.Database.CanConnect())
                {
                    MessageBox.Show("Не удалось подключиться к базе данных!\n\nПроверьте:\n1. Запущен ли SQL Server\n2. Правильная ли строка подключения\n3. Существует ли база данных RestaurantDB",
                        "Ошибка подключения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                context.Database.EnsureCreated();
                Console.WriteLine("✅ Подключение к базе данных успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД:\n\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new LoginForm(serviceProvider));
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Строка подключения - ИЗМЕНИТЕ ПОД СВОИ ДАННЫЕ!
            var connectionString = "Server=DESKTOP-BMNSL6Q;Database=RestaurantDB;Integrated Security=True;TrustServerCertificate=True;";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<UserRepository>();
            services.AddScoped<OrderRepository>();
            services.AddScoped<AuthService>();
            services.AddScoped<OrderService>();
        }
    }
}