using Models;

namespace ClientAdmin.Services.Authentication
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Function used to send the login/register request to the API and authenticate the user if it is successful
        /// </summary>
        public Task<Response<string>> AuthenticateAsync<T>(T authenticationRequest);

        /// <summary>
        /// Function used to logout the user and delete the JWT from local storage
        /// </summary>
        public Task LogoutAsync();
    }
}
