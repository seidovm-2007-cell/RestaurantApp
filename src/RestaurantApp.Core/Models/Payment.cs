namespace RestaurantApp.Core.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;

        public virtual Order? Order { get; set; }
    }
}