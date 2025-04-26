using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.OrderDTO;
using AllFoods.Core.ServiceContracts.IOrdersService;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Services.OrdersService
{
    public class OrdersGetterService : IOrdersGetterService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IMapper _mapper;

        public OrdersGetterService(IOrdersRepository ordersRepository, IMapper mapper)
        {
            _ordersRepository = ordersRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderDTO>> GetAllOrders()
        {
            List<Order> orders = await _ordersRepository.GetAllOrders();
            List<OrderDTO> orderDTO = _mapper.Map<List<OrderDTO>>(orders);
            return orderDTO;
        }

        public async Task<OrderDTO> GetOrderByID(int orderID)
        {
            Order? order = await _ordersRepository.GetOrderByID(orderID);
            if (order is null)
                throw new Exception("Order not found");
            OrderDTO orderDTO = _mapper.Map<OrderDTO>(order);
            return orderDTO;
        }
    }
}
