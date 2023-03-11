namespace Backend.src.Models
{
    public class OrderItem: BaseModel
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}