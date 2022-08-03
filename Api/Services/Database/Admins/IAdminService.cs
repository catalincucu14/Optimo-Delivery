using Models;
using Models.Authentication;
using Models.Resources;

namespace Api.Services.Database.Admins
{
    public interface IAdminService
    {
        /// <summary>
        /// Function used to check if an admin exists based on the id
        /// </summary>
        public Task<bool> ExistsAsync(int adminId);

        /// <summary>
        /// Function used to check if the credentials match an existing admin account
        /// </summary>
        public Task<Response<Admin>> AuthenticateAsync(LoginRequest loginRequest);

        /// <summary>
        /// Function used to read one admin from the database
        /// </summary>
        public Task<Response<Admin>> ReadOneAsync(int adminId);

        /// <summary>
        /// Function used to create an admin
        /// </summary>
        public Task<Response<Admin>> CreateAsync(RegisterAdminRequest registerRequest);
    }
}
