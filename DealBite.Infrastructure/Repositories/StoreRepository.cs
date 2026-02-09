using DealBite.Application.Interfaces.Repositories;
using DealBite.Domain.Entities;
using DealBite.Domain.ValueObjects;
using DealBite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace DealBite.Infrastructure.Repositories
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        public StoreRepository(ApplicationDbContext context) : base(context)
        {
        }

        public new async Task<Store> AddAsync(Store entity)
        {
            
            foreach (var location in entity.Locations)
            {
                var point = new Point(location.Coordinates.Longitude, location.Coordinates.Latitude)
                { SRID = 4326 };

                _context.Entry(location).Property<Point>("Coordinates").CurrentValue = point;
            }

            return await base.AddAsync(entity);
        }
        public async Task<IEnumerable<StoreLocation>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusInMeters)
        {
            var userLocation = new Point(longitude, latitude) { SRID = 4326 };

            var dbResults = await _context.StoreLocations
                .Include(sl=>sl.Store)
                .Where(sl=>EF.Property<Point>(sl, "Coordinates").IsWithinDistance(userLocation, radiusInMeters))
                .OrderBy(sl=>EF.Property<Point>(sl, "Coordinates").Distance(userLocation))
                .ToListAsync();

            foreach (var loc in dbResults)
            {
                var point=_context.Entry(loc).Property<Point>("Coordinates").CurrentValue;
                if(point != null)
                {
                    loc.Coordinates=new GeoCoordinate(point.Y, point.X);
                }
            }
            return dbResults;
        }
    }
}
