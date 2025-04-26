using AllFoods.Core.DTO.CartDTO;
using AllFoods.Core.DTO.CartItemDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.ICartsService
{
    public interface ICartsAdderService
    {
        Task AddToCart(CartItemRequestDTO sendCartItemDTO, string userId);

        Task<QuantityDTO> IncrementItemQuantity(Guid? productID, string userID);
    }
}
