using Models;
using Models.Resources;

namespace ClientAdmin.Services.Api.Admins
{
    public interface IAdminService
    {
        /// <summary>
        /// Function used to get an admin using the API
        /// </summary>
        public Task<Response<Admin>> GetAsync();
    }
}
