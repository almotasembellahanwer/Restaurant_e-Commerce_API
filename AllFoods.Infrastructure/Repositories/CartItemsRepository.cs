using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Infrastructure.Repositories
{
    public class CartItemsRepository : ICartItemsRepository
    {
        private readonly ApplicationDbContext _db;

        public CartItemsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void AddCartItem(CartItem cartItem)
        {
            _db.CartItems.Add(cartItem);

        }

        public async Task<CartItem?> GetCartItem(Expression<Func<CartItem, bool>> pridicates, bool tracked = false)
        {
            CartItem? cartItem;
            if (tracked == false)
            {
                 cartItem = await _db.CartItems
                .Include(temp => temp.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(pridicates);
            }
            else
            {
                cartItem = await _db.CartItems
                .Include(temp => temp.Product)
                .FirstOrDefaultAsync(pridicates);
            }
            
            if (cartItem is null)
                return null;
            return cartItem;
        }

        public void RemoveCartItem(CartItem cartItem)
        {
            _db.CartItems.Remove(cartItem);
        }
        public void RemoveRangeCartItems(List<CartItem> cartItems)
        {
            _db.CartItems.RemoveRange(cartItems);
        }
        public async Task<bool> Save()
        {
            int rowsAffected  = await _db.SaveChangesAsync();
            return rowsAffected > 0;
        }
    }
}
