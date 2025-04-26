using AllFoods.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Domain.Entities
{
    public class Cart
    {
        public Guid CartID { get; set; }
        public List<CartItem> Items { get; set; } = default!;


        public string UserID { get; set; } = string.Empty;
        [ForeignKey(nameof(UserID))]
        public ApplicationUser User { get; set; } = default!;
        public decimal TotalAmount => CalculateTotal();

        private decimal CalculateTotal()
        {
            return Items.Sum(item => item.Product.Price * item.Quantity);
        }
    }
}
