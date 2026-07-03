using System;
using System.Collections.Generic;

namespace RestaurantApp.Core.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string FullName => $"{LastName} {FirstName}{(string.IsNullOrEmpty(MiddleName) ? "" : " " + MiddleName)}";
        public int RoleID { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Role? Role { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}