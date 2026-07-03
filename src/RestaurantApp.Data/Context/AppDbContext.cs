using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Models;

namespace RestaurantApp.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ShiftTable> ShiftTables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-BMNSL6Q;Database=RestaurantDB;Integrated Security=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================================
            // РОЛИ
            // ============================================================
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleID);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.RoleName).IsUnique();
            });

            // ============================================================
            // ПОЛЬЗОВАТЕЛИ
            // ============================================================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.Login).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MiddleName).HasMaxLength(50);
                entity.HasIndex(e => e.Login).IsUnique();

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // СТОЛЫ
            // ============================================================
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(e => e.TableID);
                entity.Property(e => e.TableNumber).IsRequired();
                entity.Property(e => e.Capacity).IsRequired().HasDefaultValue(4);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Свободен");
                entity.HasIndex(e => e.TableNumber).IsUnique();
            });

            // ============================================================
            // БЛЮДА
            // ============================================================
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.MenuItemID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.StockQuantity).IsRequired().HasDefaultValue(0);
            });

            // ============================================================
            // СМЕНЫ
            // ============================================================
            modelBuilder.Entity<Shift>(entity =>
            {
                entity.HasKey(e => e.ShiftID);
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Shifts)
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // СМЕНА-СТОЛ
            // ============================================================
            modelBuilder.Entity<ShiftTable>(entity =>
            {
                entity.HasKey(e => e.ShiftTableID);
                entity.HasIndex(e => new { e.ShiftID, e.TableID }).IsUnique();

                entity.HasOne(e => e.Shift)
                    .WithMany(s => s.ShiftTables)
                    .HasForeignKey(e => e.ShiftID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Table)
                    .WithMany(t => t.ShiftTables)
                    .HasForeignKey(e => e.TableID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // БРОНИРОВАНИЯ
            // ============================================================
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.ReservationID);
                entity.Property(e => e.TableID).IsRequired();
                entity.Property(e => e.UserID).IsRequired();
                entity.Property(e => e.ReservationDate).IsRequired();
                entity.Property(e => e.ReservationTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.GuestCount).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Активна");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                // Связь с Tables
                entity.HasOne(e => e.Table)
                    .WithMany(t => t.Reservations)
                    .HasForeignKey(e => e.TableID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Связь с Users
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Reservations)
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                // ============================================================
                // НЕТ СВЯЗИ С ORDER!
                // ============================================================
            });

            // ============================================================
            // ЗАКАЗЫ - НЕТ СВЯЗИ С RESERVATION!
            // ============================================================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderID);
                entity.Property(e => e.ReservationID).IsRequired(false);
                entity.Property(e => e.ShiftID).IsRequired();
                entity.Property(e => e.OrderStatus).IsRequired().HasMaxLength(30).HasDefaultValue("Составление");
                entity.Property(e => e.OrderDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TotalSum).IsRequired().HasPrecision(10, 2).HasDefaultValue(0);

                // Связь с Shift
                entity.HasOne(e => e.Shift)
                    .WithMany(s => s.Orders)
                    .HasForeignKey(e => e.ShiftID)
                    .OnDelete(DeleteBehavior.Restrict);

                // ============================================================
                // ВАЖНО: НЕТ СВЯЗИ С RESERVATION!
                // ============================================================
            });

            // ============================================================
            // ПОЗИЦИИ ЗАКАЗА
            // ============================================================
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemID);
                entity.Property(e => e.OrderID).IsRequired();
                entity.Property(e => e.MenuItemID).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.Discount).HasPrecision(5, 2).HasDefaultValue(0);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MenuItem)
                    .WithMany(m => m.OrderItems)
                    .HasForeignKey(e => e.MenuItemID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // ОПЛАТА
            // ============================================================
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentID);
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Amount).IsRequired().HasPrecision(10, 2);
                entity.Property(e => e.ReceiptNumber).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.ReceiptNumber).IsUnique();

                entity.HasOne(e => e.Order)
                    .WithOne(o => o.Payment)
                    .HasForeignKey<Payment>(e => e.OrderID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // АКЦИИ
            // ============================================================
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.HasKey(e => e.PromotionID);
                entity.Property(e => e.DiscountPercent).IsRequired().HasPrecision(5, 2);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(false);

                entity.HasOne(e => e.MenuItem)
                    .WithMany(m => m.Promotions)
                    .HasForeignKey(e => e.MenuItemID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // ИНДЕКСЫ
            // ============================================================
            modelBuilder.Entity<Order>().HasIndex(e => e.OrderStatus);
            modelBuilder.Entity<Order>().HasIndex(e => e.OrderDate);
            modelBuilder.Entity<Reservation>().HasIndex(e => e.ReservationDate);
            modelBuilder.Entity<Shift>().HasIndex(e => e.IsActive);
        }
    }
}