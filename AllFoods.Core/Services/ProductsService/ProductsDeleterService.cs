using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Core.DTO.CartDTO;
using AllFoods.Core.Helpers;
using AllFoods.Core.ServiceContracts.IProductsService;
using AllFoods.Core.Settinges;
using AutoMapper;

namespace AllFoods.Core.Services.ProductsService
{
    public class ProductsDeleterService : IProductsDeleterService
    {

        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;
        private readonly string _imagePath;

        public ProductsDeleterService(IMapper mapper, IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _imagePath = $"wwwroot{FileSettings.ImagesPath}";
        }

        public async Task<bool> DeleteProduct(Guid? productID)
        {
            if(productID is null)
            {
                throw new ArgumentNullException("Given person id doesn't exist");
            }
            Product? productByID = await _productsRepository.GetProductByProductID(productID.Value);
            if(productByID is null)
            {
                return false;
            }
            DeleteCoverHelper.DeleteFile(productByID.ImageUrl, "products");
            bool isDeleted = await _productsRepository.DeleteProductByProductID(productID);
            return true;
                
        }


    }
}
