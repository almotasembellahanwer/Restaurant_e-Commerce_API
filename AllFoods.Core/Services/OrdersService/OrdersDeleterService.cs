using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.ServiceContracts.IOrdersService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Services.OrdersService
{
    public class OrdersDeleterService : IOrdersDeleterService
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrdersDeleterService(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public void DeleteOrder(int orderID)
        {
            _ordersRepository.DeleteOrder(orderID);
            _ordersRepository.Save();
        }
    }
}
