using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CartDTO;
using AllFoods.Core.DTO.CartItemDTO;
using AllFoods.Core.ServiceContracts.ICartsService;
using AutoMapper;

namespace AllFoods.Core.Services.CartsService
{
    public class CartsAdderService : ICartsAdderService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly ICartItemsRepository _cartItemsRepository;
        private readonly IMapper _mapper;

        public CartsAdderService(ICartsRepository cartsRepository, ICartItemsRepository cartItemsRepository, IMapper mapper)
        {
            _cartsRepository = cartsRepository;
            _cartItemsRepository = cartItemsRepository;
            _mapper = mapper;
        }

        public async Task AddToCart(CartItemRequestDTO cartItemRequestDTO, string userID)
        {
            // Get cart for a specific user from database
            Cart cart = await _cartsRepository.GetCart(userID);
            // If cart.Id equal 0 this means that the cart is empty and we need to create a new one
            if (cart.CartID == Guid.Empty)
            {
                _cartsRepository.AddCart(cart);
                await _cartsRepository.Save();
            }
            // Get existing CartItem from database
            CartItem? cartItems = await _cartItemsRepository.GetCartItem(temp => temp.CartID == cart.CartID && temp.ProductID == cartItemRequestDTO.ProductID);
            // If it is not null, we need to update the quantity of the existing CartItem
            if (cartItems != null)
            {
                cartItems.Quantity += cartItemRequestDTO.Quantity;
            }
            else
            {
                // If it is null, we need to create a new CartItem

                CartItem cartItem = new()
                {
                    CartID = cart.CartID,
                    ProductID = cartItemRequestDTO.ProductID,
                    Quantity = cartItemRequestDTO.Quantity,
                    Size = cartItemRequestDTO.Size
                };
                // Add CartItem to database
                _cartItemsRepository.AddCartItem(cartItem);
            }
            await _cartItemsRepository.Save();

        }

        public async Task<QuantityDTO> IncrementItemQuantity(Guid? productID, string userID)
        {
            // Get cart for a specific user from database
            Cart cart = await _cartsRepository.GetCart(userID);
            if (cart is null)
                throw new Exception("Cart should not be null");
            //Get CartItem from database
            CartItem? cartItem = await _cartItemsRepository.GetCartItem(temp => temp.CartID == cart.CartID && temp.ProductID == productID,true);

            if (cartItem is null)
                throw new Exception("CartItem should not be null");
            cartItem.Quantity++;
            bool isAffected = await _cartItemsRepository.Save();

            QuantityDTO quantityDTO = new()
            {
                ProductID = productID
            };

            return quantityDTO;

        }
    }
}
