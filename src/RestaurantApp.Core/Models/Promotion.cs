namespace RestaurantApp.Core.Models
{
    public class Promotion
    {
        public int PromotionID { get; set; }
        public int? MenuItemID { get; set; }
        public string? Category { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public virtual MenuItem? MenuItem { get; set; }
    }
}