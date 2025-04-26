using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CartDTO;
using AllFoods.Core.DTO.CartItemDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.ICartsService
{
    public interface ICartsDeleterService
    {
        Task DeleteFromCart(Guid? productID, string userID);
        public Task<QuantityDTO> DecrementItemQuantity(Guid? productID, string userID);
    }
}
