using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.IdentityEntities;
using AllFoods.Core.DTO.AccountDTO;
using AllFoods.Core.DTO.CartItemDTO;
using AllFoods.Core.DTO.CategoyDTO;
using AllFoods.Core.DTO.OrderDTO;
using AllFoods.Core.DTO.ProductDTO;
using AllFoods.Core.DTO.UserDTO;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Automapper;
namespace AllFoods.Core.MappingCofig
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Category,CategoryAddRequest>().ReverseMap();
            CreateMap<CategoryResponse, Category>().ReverseMap();

            CreateMap<CategoryAddRequest, CategoryResponse>().ReverseMap();

            CreateMap<CategoryResponse, APIResponse>().ReverseMap();


            CreateMap<Product, ProductAddRequest>().ReverseMap();
            CreateMap<ProductResponse, Product>().ReverseMap();
            CreateMap<ProductResponse, ProductAddRequest>().ReverseMap();


            CreateMap<Product, ProductUpdateRequest>().ReverseMap();


            CreateMap<AccountDTO, ApplicationUser>().ReverseMap();

            CreateMap<CartItemResponseDTO, CartItem>().ReverseMap();
            CreateMap<CartItem, CartItemResponseDTO>().ReverseMap();


            CreateMap<CartItemRequestDTO, CartItemResponseDTO>().ReverseMap();


            CreateMap<OrderItem, CartItem>().ReverseMap();

            CreateMap<OrderDTO, Order>().ReverseMap();

            CreateMap<UserDTO, ApplicationUser>().ReverseMap();



        }
    }
}
