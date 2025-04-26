using AllFoods.Core.DTO.CartDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.ServiceContracts.ICartsService
{
    public interface ICartsGetterService
    {
        Task<CartDTO> GetCart(string userID);
    }
}
