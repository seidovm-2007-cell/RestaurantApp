using System.Collections.Generic;

namespace RestaurantApp.Core.Models
{
    public class Table
    {
        public int TableID { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; } = 4;
        public string Status { get; set; } = "Свободен";

        public virtual ICollection<ShiftTable> ShiftTables { get; set; } = new List<ShiftTable>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}