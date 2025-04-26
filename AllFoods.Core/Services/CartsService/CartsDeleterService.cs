using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CartDTO;
using AllFoods.Core.DTO.CartItemDTO;
using AllFoods.Core.ServiceContracts.ICartsService;

namespace AllFoods.Core.Services.CartsService
{
    public class CartsDeleterService : ICartsDeleterService
    {
        private readonly ICartsRepository _cartsRepository;
        private readonly ICartItemsRepository _cartItemsRepository;


        public CartsDeleterService(ICartsRepository cartsRepository, ICartItemsRepository cartItemsRepository)
        {
            _cartsRepository = cartsRepository;
            _cartItemsRepository = cartItemsRepository;
        }

        public async Task DeleteFromCart(Guid? productID, string userID)
        {
            if (productID is null)
                throw new Exception("productID should not be null");
            // Get cart for a specific user from database
            Cart? cart = await _cartsRepository.GetCart(userID);
            if (cart == null)
                throw new Exception("Cart not found");
            // Get existing CartItem from database
            CartItem? cartItem = await _cartItemsRepository.GetCartItem(temp => temp.CartID == cart.CartID && temp.ProductID == productID);
            if (cartItem is null)
                throw new Exception("CartItem not found");

            // Reomove CartItem from database
            _cartItemsRepository.RemoveCartItem(cartItem);
            await _cartItemsRepository.Save();
        }

        public async Task<QuantityDTO> DecrementItemQuantity(Guid? productID, string userID)
        {
            // Get cart for a specific user from database
            Cart cart = await _cartsRepository.GetCart(userID);
            if (cart is null)
                throw new Exception("Cart should not be null");
            //Get CartItem from database
            CartItem? cartItem = await _cartItemsRepository.GetCartItem(temp => temp.CartID == cart.CartID && temp.ProductID == productID,true);

            if (cartItem is null)
                throw new Exception("CartItem should not be null");

            if(cartItem.Quantity > 1)
            {
                cartItem.Quantity--;

            }
            else
            {
                _cartItemsRepository.RemoveCartItem(cartItem);
            }

            await _cartItemsRepository.Save();
            QuantityDTO quantityDTO = new()
            {
                ProductID = productID
            };

            return quantityDTO;

        }
    }
}
