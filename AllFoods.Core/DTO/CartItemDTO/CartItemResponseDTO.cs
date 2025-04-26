using AllFoods.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.DTO.CartItemDTO
{
    public class CartItemResponseDTO
    {
        public Guid ProductID { get; set; }
        [MaxLength(50)]
        public string ProductName { get; set; } = string.Empty;
        [Required(ErrorMessage = "{0} should not be blank")]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPriceForProduct { get; set; }
    }
}
