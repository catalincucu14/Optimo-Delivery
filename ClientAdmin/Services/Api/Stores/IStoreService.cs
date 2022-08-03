using Models;
using Models.Resources;

namespace ClientAdmin.Services.Api.Stores
{
    public interface IStoreService
    {
        /// <summary>
        /// Function used to get a store using the API
        /// </summary>
        public Task<Response<Store>> GetAsync();

        /// <summary>
        /// Function used to create a store using the API
        /// </summary>
        public Task<Response<Store>> CreateAsync(Store store);

        /// <summary>
        /// Function used to update a store using the API
        /// </summary>
        public Task<Response<Store>> UpdateAsync(Store store);
    }
}
