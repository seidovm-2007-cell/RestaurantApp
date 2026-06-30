namespace RestaurantApp.Core.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public int TableID { get; set; }
        public int UserID { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int GuestCount { get; set; }
        public string Status { get; set; } = "Активна";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Table? Table { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}