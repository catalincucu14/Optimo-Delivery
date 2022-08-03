using Models;
using Models.Authentication;
using Models.Resources;

namespace ClientAdmin.Services.Api.Drivers
{
    public interface IDriverService
    {
        /// <summary>
        /// Function used to get drivers using the API
        /// </summary>
        public Task<Response<List<Driver>>> GetAsync();

        /// <summary>
        /// Function used to create a driver using the API
        /// </summary>
        public Task<Response<Driver>> CreateAsync(RegisterDriverRequest driver);

        /// <summary>
        /// Function used to update a driver using the API
        /// </summary>
        public Task<Response<Driver>> UpdateAsync(RegisterDriverRequest driver, int id);
    }
}
