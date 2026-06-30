namespace RestaurantApp.Core.Models
{
    public class Shift
    {
        public int ShiftID { get; set; }
        public int UserID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual User? User { get; set; }
        public virtual ICollection<ShiftTable> ShiftTables { get; set; } = new List<ShiftTable>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}