using AutoMapper;
using DealBite.Application.DTOs;
using DealBite.Application.Interfaces.Repositories;
using MediatR;

namespace DealBite.Application.Features.Stores.Queries.GetNearbyStores
{
    public class GetNearbyStoresQuery : IRequest<List<StoreLocationDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusInMeters { get; set; } = 5000;
    }
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

            foreach (var dto in dtos)
            {
                var distance = CalculateDistanceInMeters(
                    request.Latitude, request.Longitude,
                    dto.Latitude, dto.Longitude);

                dto.DistanceInMeters = Math.Round(distance, 0);
            }

            return dtos;
        }

        /// <summary>
        /// Haversine formula a két GPS koordináta közötti távolság kiszámítására méterben.
        /// (Ez volt eddig a GeoCoordinate osztályban)
        /// </summary>
        private static double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // Föld sugara méterben
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}