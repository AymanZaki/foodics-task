using AutoMapper;
using Entities.Entities;
using Models.Contracts;
using Models.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Orders.API.MappingProfiles
{
    public class OrdersMappingProfile : Profile
    {
        public OrdersMappingProfile()
        {
            #region Map Dto to Entity
            MapCreateOrderDTOToOrderEntity();
            #endregion

            #region Map Entity to Dto
            MapProductEntityToProductDTO();
            MapProductIngredientEntityToProductIngredientDTO();
            MapIngredientEntityToIngredientDTO();
            MapIngredientStockEntityToIngredientStockDTO();
            MapOrderEntityToOrderDTO();
            MapOrderProductEntityToOrderProductDTO();
            #endregion

            #region Map Dto to Response
            MapOrderResponseToOrderDTO();
            #endregion
        }

        #region Map Entity To Dto
        void MapProductEntityToProductDTO()
        {
            CreateMap<Product, ProductDTO>()
           ;
        }
        void MapProductIngredientEntityToProductIngredientDTO()
        {
            CreateMap<ProductIngredient, ProductIngredientDTO>()
           ;
        }
        void MapIngredientEntityToIngredientDTO()
        {
            CreateMap<Ingredient, IngredientDTO>()
           ;
        }
        void MapIngredientStockEntityToIngredientStockDTO()
        {
            CreateMap<IngredientStock, IngredientStockDTO>()
           ;
        }
        void MapOrderEntityToOrderDTO()
        {
            CreateMap<Order, OrderDTO>()
           ;
        }
        void MapOrderProductEntityToOrderProductDTO()
        {
            CreateMap<OrderProduct, OrderProductDTO>()
           ;
        }
        #endregion


        #region Map Dto to Entity
        void MapCreateOrderDTOToOrderEntity()
        {
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.Products))
            ;

            CreateMap<OrderProductRequestDTO, OrderProduct>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product_Id))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))

            ;
        }
        #endregion

        #region Map Dto to Response
        void MapOrderResponseToOrderDTO()
        {
            CreateMap<OrderResponse, OrderDTO>()
            ;
        }
        #endregion
    }
}
