using Models;
using Models.Resources;

namespace Api.Services.Database.Stores
{
    public interface IStoreService
    {
        /// <summary>
        /// Function used to read a store from the database
        /// </summary>
        public Task<Response<Store>> ReadOneAsync(int adminId);

        /// <summary>
        /// Function used to create a store
        /// </summary>
        public Task<Response<Store>> CreateAsync(int adminId, Store store);

        /// <summary>
        /// Function used to update a store
        /// </summary>
        public Task<Response<Store>> UpdateAsync(int adminId, Store store);
    }
}
