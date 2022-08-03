using Models;
using Route = Models.Resources.Route;

namespace Api.Services.Database.Routes
{
    public interface IRouteService
    {
        /// <summary>
        /// Function used to check if a route exists based on the id
        /// </summary>
        public Task<bool> ExistsAsync(int routeId, int adminId);

        /// <summary>
        /// Function used to read the routes from the database
        /// </summary>
        public Task<Response<List<Route>>> ReadAllAsync(int adminId);

        /// <summary>
        /// Function used to create a route
        /// </summary>
        public Task<Response<Route>> CreateAsync(int adminId, Route route);

        /// <summary>
        /// Function used to update a route
        /// </summary>
        public Task<Response<Route>> UpdateAsync(int routeId, int adminId, Route route);

        /// <summary>
        /// Function used to delete a route
        /// </summary>
        public Task<Response<Route>> DeleteAsync(int routeId, int adminId);
    }
}
