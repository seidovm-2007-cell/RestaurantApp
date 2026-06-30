namespace RestaurantApp.Core.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int MenuItemID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }

        public virtual Order? Order { get; set; }
        public virtual MenuItem? MenuItem { get; set; }
    }
}