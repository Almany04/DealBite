using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Domain.Entities;

namespace DealBite.Application.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<StoreLocation, StoreLocationDto>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.Store != null ? src.Store.LogoUrl : string.Empty))

                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Coordinates.Y))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Coordinates.X))

                .ForMember(dest => dest.DistanceInMeters, opt => opt.Ignore());
            CreateMap<ProductPrice, ProductPriceDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
                .ForMember(dest => dest.OldPrice, opt => opt.MapFrom(src => src.OriginalPrice != null ? src.OriginalPrice.Value.Amount : (decimal?)null))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : "Ismeretlen"))
                .ForMember(dest => dest.PriceSource, opt => opt.MapFrom(src => src.Source.ToString())); 

            
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.AiGeneratedImageUrl))
                
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.UnitType.ToString()))
               
                .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.Prices));

            CreateMap<PriceHistory, PriceHistoryDto>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name:"Ismeretlen"));

            CreateMap<Store, StoreDto>();
            CreateMap<Category, CategoryDto>();
                
        }
    }
    
}
