using Models;
using Models.Authentication;
using Models.Resources;

namespace Api.Services.Database.Drivers
{
    public interface IDriverService
    {
        /// <summary>
        /// Function used to check if a driver exists based on the id
        /// </summary>
        public Task<bool> ExistsAsync(int driverId, int adminId);

        /// <summary>
        /// Function used to check if the credentials match an existing driver account
        /// </summary>
        public Task<Response<Driver>> AuthenticateAsync(LoginRequest loginRequest);

        /// <summary>
        /// Function used to read the drivers from the database
        /// </summary>
        public Task<Response<List<Driver>>> ReadAllAsync(int adminId);

        /// <summary>
        /// Function used to create a driver
        /// </summary>
        public Task<Response<Driver>> CreateAsync(int adminId, RegisterDriverRequest registerRequest);

        /// <summary>
        /// Function used to update a driver
        /// </summary>
        public Task<Response<Driver>> UpdateAsync(int driverId, int adminId, RegisterDriverRequest registerRequest);
    }
}
