using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.DTO.CartItemDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.DTO.CartDTO
{
    public class CartDTO
    {
        public string UserID { get; set; } = string.Empty;
        public List<CartItemResponseDTO> Items { get; set; } = default!;
        public decimal TotalAmount { get; set; }
    }
}
