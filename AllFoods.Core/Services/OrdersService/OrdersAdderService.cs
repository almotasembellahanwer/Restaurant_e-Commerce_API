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
    public class OrdersAdderService : IOrdersAdderService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly ICartItemsRepository _cartItemsRepository;

        private readonly IOrdersRepository _ordersRepository;

        private readonly IMapper _mapper;


        public OrdersAdderService(ICartsRepository cartsRepository, IMapper mapper, IOrdersRepository ordersRepository, ICartItemsRepository cartItemsRepository)
        {
            _cartsRepository = cartsRepository;
            _mapper = mapper;
            _ordersRepository = ordersRepository;
            _cartItemsRepository = cartItemsRepository;
        }

        public async Task<OrderDTO> CreateOrder(string userID)
        {
            // Get cart from database
            Cart? cart = await _cartsRepository.GetCart(userID);
            if (cart is null)
                throw new Exception("Cart is empty");
            // Create a new Order object
            Order order = new Order()
            {
                UserID = userID,
                DateTime = DateTime.UtcNow,
                status = Enums.OrderStatus.Pending,
                subtotal = cart.TotalAmount,
                OrderItems = _mapper.Map<List<OrderItem>>(cart.Items)
            };
            // Convert it to OrderDTO
            OrderDTO orderDTO = _mapper.Map<OrderDTO>(order);
            // Add order to Orders table in database
            _ordersRepository.AddOrder(order);
            // remove items from cart
            _cartItemsRepository.RemoveRangeCartItems(cart.Items);
            await _cartsRepository.Save();

            return orderDTO;
        }
    }
}
