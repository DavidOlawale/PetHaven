namespace PetHaven.BusinessLogic.DTOs
{
    public class CreateOrderDTO
    {
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public int UserId { get; set; }
        public List<CreateOrderItemDTO> Items { get; set; } = new List<CreateOrderItemDTO>();
    }

    public class CreateOrderItemDTO
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
