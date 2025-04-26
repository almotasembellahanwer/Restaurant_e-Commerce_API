using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CartDTO;
using AllFoods.Core.DTO.CartItemDTO;
using AllFoods.Core.ServiceContracts.ICartsService;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllFoods.Core.Services.CartsService
{
    public class CartsGetterService : ICartsGetterService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly IMapper _mapper;

        public CartsGetterService(ICartsRepository cartsRepository, IMapper mapper)
        {
            _cartsRepository = cartsRepository;
            _mapper = mapper;
        }

        public async Task<CartDTO> GetCart(string userID)
        {
            // Get cart for a specific user from database
            Cart? cart = await _cartsRepository.GetCart(userID);
            //_mapper.Map<List<CartItemResponseDTO>>(cart.Items)
            // Map to CartDTO
            CartDTO cartDTO = new CartDTO()
            {
                UserID = cart.UserID,
                Items = cart.Items.Select(item => new CartItemResponseDTO
                {
                    ProductID = item.ProductID,
                    ProductName = item.Product.ProductName,
                    Quantity = item.Quantity,
                    Description = item.Product.Description,
                    Price = item.Product.Price,
                    ImageUrl = item.Product.ImageUrl,
                    TotalPriceForProduct = item.TotalPriceForProduct
                }).ToList(),
                TotalAmount = cart.TotalAmount
            };

            // Store full path to imageUrl
            return cartDTO;
        }
    }
}
