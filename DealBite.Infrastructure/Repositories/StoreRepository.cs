using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealBite.Infrastructure.Repositories
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        public StoreRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Store?> GetByIdWithLocationsAsync(Guid storeId)
        {
            return await _context.Stores
                 .AsNoTracking()
                 .Include(sl => sl.Locations)
                 .FirstOrDefaultAsync(s => s.Id == storeId);
        }
        public async Task<List<Store>> GetAllWithLocationsAsync()
        {
            return await _context.Stores
                .AsNoTracking()
                .Include(sl => sl.Locations)
                .ToListAsync();
        }

        public async Task<IEnumerable<StoreLocation>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusInMeters)
        {
            var userLocation = new Point(longitude, latitude) { SRID = 4326 };

            return await _context.StoreLocations
                .Include(sl => sl.Store)
                .Where(sl => sl.Coordinates.IsWithinDistance(userLocation, radiusInMeters))
                .OrderBy(sl => sl.Coordinates.Distance(userLocation))
                .ToListAsync();
        }
    }
}