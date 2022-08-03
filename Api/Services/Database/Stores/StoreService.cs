using Api.Database;
using Api.Services.Map;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Resources;
using Models.Utils;

#nullable disable

namespace Api.Services.Database.Stores
{
    public class StoreService : IStoreService
    {
        private readonly DatabaseContext _databaseContext;

        private readonly IMapService _mapboxService;

        public StoreService(DatabaseContext databaseContext, IMapService mapboxService)
        {
            _databaseContext = databaseContext;

            _mapboxService = mapboxService;
        }

        /// <inheritdoc />
        public async Task<Response<Store>> ReadOneAsync(int adminId)
        {
            Store store = await _databaseContext.Stores
                .Where(storeTemp => storeTemp.AdminId == adminId)
                .FirstOrDefaultAsync();

            return new Response<Store>
            {
                Success = true,
                Data = store
            };
        }

        /// <inheritdoc />
        public async Task<Response<Store>> CreateAsync(int adminId, Store store)
        {
            // Get the country of the admin
            string country = (await _databaseContext.Admins
                .Where(adminTemp => adminTemp.AdminId == adminId)
                .FirstAsync())
                .Country;

            // Get the coordinates of the location
            Coordinates coodinates = _mapboxService.GetCoordinates(country, store.City, store.Address);

            // Set the coordinates to the store
            store.Latitude = coodinates.Latitude;
            store.Longitude = coodinates.Longitude;

            // Create the store
            await _databaseContext.Stores.AddAsync(store);

            // Save changes
            await _databaseContext.SaveChangesAsync();

            return new Response<Store>
            {
                Success = true,
                Data = store
            };
        }

        /// <inheritdoc />
        public async Task<Response<Store>> UpdateAsync(int adminId, Store store)
        {
            // Get the country of the admin
            string country = (await _databaseContext.Admins
                .Where(adminTemp => adminTemp.AdminId == adminId)
                .FirstAsync())
                .Country;

            // Get the store
            Store storeVar = await _databaseContext.Stores
                .Where(storeTemp => storeTemp.AdminId == adminId)
                .FirstOrDefaultAsync();

            // Get the coordinates of the location from Mapbox API
            Coordinates coordinates = _mapboxService.GetCoordinates(country, store.City, store.Address);

            // Set the coordinates to the store
            store.Latitude = coordinates.Latitude;
            store.Longitude = coordinates.Longitude;

            // Update the store
            storeVar.City = store.City;
            storeVar.Address = store.Address;

            // Save changes
            await _databaseContext.SaveChangesAsync();

            return new Response<Store>
            {
                Success = true,
                Data = store
            };
        }
    }
}
