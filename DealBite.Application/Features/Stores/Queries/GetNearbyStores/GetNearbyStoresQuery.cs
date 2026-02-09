using DealBite.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Features.Stores.Queries.GetNearbyStores
{
    public class GetNearbyStoresQuery:IRequest<List<StoreLocationDto>>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusInMeters { get; set; } = 5000;
    }
}
