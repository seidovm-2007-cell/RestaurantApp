namespace RestaurantApp.Core.Models
{
    public class ShiftTable
    {
        public int ShiftTableID { get; set; }
        public int ShiftID { get; set; }
        public int TableID { get; set; }

        public virtual Shift? Shift { get; set; }
        public virtual Table? Table { get; set; }
    }
}