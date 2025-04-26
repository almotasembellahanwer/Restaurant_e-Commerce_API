using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Infrastructure.Repositories
{
    public class CartsRepository : ICartsRepository
    {
        private readonly ApplicationDbContext _db;

        public CartsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Cart> GetCart(string userID)
        {
            Cart? cart = await _db.Carts
                .Include(temp => temp.Items)
                .ThenInclude(temp => temp.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(temp => temp.UserID == userID);
            if (cart is null)
            {
                cart = new Cart()
                {
                    UserID = userID
                };
            }
            return cart;
        }
        public void AddCart(Cart cart)
        {
            _db.Carts.Add(cart);
        }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
