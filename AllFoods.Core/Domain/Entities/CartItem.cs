using System.ComponentModel.DataAnnotations.Schema;
using AllFoods.Core.Enums;
namespace AllFoods.Core.Domain.Entities
{
    public class CartItem
    {
        [ForeignKey(nameof(Cart))]
        public Guid CartID { get; set; }
        public Cart Cart { get; set; } = default!;
        [ForeignKey(nameof(Product))]
        public Guid ProductID { get; set; }
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
        public Size Size { get; set; }
        public decimal TotalPriceForProduct  => TotalPrice();

        private decimal TotalPrice()
        {
            return Product.Price * Quantity;
        }
    }
}
