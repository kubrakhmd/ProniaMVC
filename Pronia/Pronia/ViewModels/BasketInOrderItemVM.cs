namespace Pronia.ViewModels
{
    public class BasketInOrderItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public int Count { get; set; }
    }
}
