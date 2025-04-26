using AllFoods.Core.Enums;

namespace AllFoods.Core.DTO.CartItemDTO
{
    public class CartItemRequestDTO
    {
        public Guid ProductID { get; set; }
        public int Quantity { get; set; }
        public Size Size { get; set; }
    }
}
