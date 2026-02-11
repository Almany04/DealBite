using DealBite.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Application.Interfaces.Repositories
{
    public interface IStoreRepository:IGenericRepository<Store>
    {
        Task<IEnumerable<StoreLocation>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusInMeters); 
    }
}
