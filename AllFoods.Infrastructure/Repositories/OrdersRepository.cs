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
    public class OrdersRepository : IOrdersRepository
    {
        private readonly ApplicationDbContext _db;

        public OrdersRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public void AddOrder(Order order)
        {
            _db.Orders.Add(order);
        }

        public async Task DeleteOrder(int orderID)
        {
            Order? order = await GetOrderByID(orderID);
            if (order is null)
                throw new Exception("Order not found");
            _db.Orders.Remove(order);
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _db.Orders
                .Include(temp=>temp.User)
                .Include(temp => temp.OrderItems)
                .ThenInclude(temp => temp.Product)
                .AsNoTracking().ToListAsync();
        }

        public async Task<Order?> GetOrderByID(int? orderID)
        {
            Order? order = await _db.Orders
                .Include(temp => temp.OrderItems)
                .ThenInclude(temp=>temp.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(temp => temp.Id == orderID);
            if (order is null)
                return null;
            return order;
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
