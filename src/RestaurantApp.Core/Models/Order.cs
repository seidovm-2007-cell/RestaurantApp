namespace RestaurantApp.Core.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int? ReservationID { get; set; }
        public int ShiftID { get; set; }
        public string OrderStatus { get; set; } = "Составление";
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalSum { get; set; }

        public virtual Reservation? Reservation { get; set; }
        public virtual Shift? Shift { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual Payment? Payment { get; set; }
    }
}