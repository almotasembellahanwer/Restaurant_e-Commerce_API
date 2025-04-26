using AllFoods.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Domain.RepositoryContracts
{
    public interface ICartsRepository
    {
        Task<Cart> GetCart(string userID);
        void AddCart(Cart cart);
        Task Save();
    }
}
