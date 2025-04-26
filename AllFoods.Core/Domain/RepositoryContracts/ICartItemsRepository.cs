using AllFoods.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Domain.RepositoryContracts
{
    public interface ICartItemsRepository
    {
        void AddCartItem(CartItem cartItem);
        Task<CartItem?> GetCartItem(Expression<Func<CartItem,bool>> pridicates, bool tracked = false);
        void RemoveCartItem(CartItem cartItem);
        void RemoveRangeCartItems(List<CartItem> cartItems);

        Task<bool> Save();

    }
}
