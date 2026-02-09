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

                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Coordinates.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Coordinates.Longitude))

                .ForMember(dest => dest.DistanceInMeters, opt => opt.Ignore());
        }
    }
}
