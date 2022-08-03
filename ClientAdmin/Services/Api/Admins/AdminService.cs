using ClientAdmin.Handlers;
using ClientAdmin.Services.Authentication;
using Models;
using Models.Resources;
using ILocalStorageService = ClientAdmin.Services.LocalStorage.ILocalStorageService;

#nullable disable

namespace ClientAdmin.Services.Api.Admins
{
    public class AdminService : IAdminService
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILocalStorageService _localStorageService;

        private readonly HttpClient _httpClient;

        public AdminService(IAuthenticationService authenticationService, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _authenticationService = authenticationService;

            _localStorageService = localStorageService;

            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<Response<Admin>> GetAsync()
        {
            ApiHandler<Admin> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a GET request to the API to get an admin 
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/admins/me");

            return await apiHandler.HandleRequestAsync(httpRequest);
        }
    }
}
