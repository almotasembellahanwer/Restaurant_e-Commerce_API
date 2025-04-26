using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllFoods.Core.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        [Column(TypeName = "nvarchar(20)")]
        public OrderStatus status { get; set; } = OrderStatus.Pending;

        public decimal DeliveryCost { get; set; } = 60;
        public decimal subtotal { get; set; }
        public decimal TotalPrice { get => DeliveryCost + subtotal; }


        public string UserID { get; set; } = string.Empty;
        [ForeignKey("UserID")]
        public ApplicationUser User { get; set; } = default!;
        public List<OrderItem> OrderItems { get; set; } = default!;
    }
}
