using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Order Order { get; set; } = default!;

        public Guid ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
