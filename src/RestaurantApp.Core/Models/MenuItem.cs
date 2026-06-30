namespace RestaurantApp.Core.Models
{
    public class MenuItem
    {
        public int MenuItemID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; } = true;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
    }
}