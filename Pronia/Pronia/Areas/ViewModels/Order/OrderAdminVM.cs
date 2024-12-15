namespace Pronia.Areas.ViewModels
{
    public class OrderAdminVM
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }

        public decimal Subtotal { get; set; }
        public bool? Status { get; set; }
        public string CreatedAt { get; set; }
    }
}
