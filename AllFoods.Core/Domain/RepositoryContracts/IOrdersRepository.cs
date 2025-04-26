using AllFoods.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Domain.RepositoryContracts
{
    public interface IOrdersRepository
    {
        void AddOrder(Order order);
        Task<List<Order>> GetAllOrders();
        Task<Order?> GetOrderByID(int? orderID);

        Task DeleteOrder(int orderID);

        Task Save();
    }
}
