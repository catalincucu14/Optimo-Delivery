using Models;
using Models.Resources;

namespace ClientAdmin.Services.Api.Routes
{
    public interface IRouteService
    {
        /// <summary>
        /// Function used to get the routes using the API
        /// </summary>
        public Task<Response<List<Route>>> GetAsync();

        /// <summary>
        /// Function used to create a route using the API
        /// </summary>
        public Task<Response<Route>> CreateAsync(Route route);

        /// <summary>
        /// Function used to update a route using the API
        /// </summary>
        public Task<Response<Route>> UpdateAsync(Route route);

        /// <summary>
        /// Function used to delete a route using the API
        /// </summary>
        public Task<Response<Route>> DeleteAsync(Route route);
    }
}
