using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.ValueObjects;
using MediatR;
namespace DealBite.Application.Features.Stores.Queries.GetNearbyStores
{
    public class GetNearbyStoresHandler : IRequestHandler<GetNearbyStoresQuery, List<StoreLocationDto>>
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public GetNearbyStoresHandler(IStoreRepository storeRepository, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _mapper = mapper;
        }
        public async Task<List<StoreLocationDto>> Handle(GetNearbyStoresQuery request, CancellationToken cancellationToken)
        {
            var locations = await _storeRepository.GetNearbyLocationsAsync(request.Latitude, request.Longitude, request.RadiusInMeters);

            var dtos = _mapper.Map<List<StoreLocationDto>>(locations);

            var userGeo = new GeoCoordinate(request.Latitude, request.Longitude);

            foreach (var dto in dtos)
            {
                var storeGeo = new GeoCoordinate(dto.Latitude, dto.Longitude);
                dto.DistanceInMeters = Math.Round(storeGeo.GetDistanceTo(userGeo) * 1000, 0);
            }

            return dtos;
        }
    }
}
