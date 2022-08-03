using ClientAdmin.Handlers;
using ClientAdmin.Services.Authentication;
using Models;
using Models.Authentication;
using Models.Resources;
using ILocalStorageService = ClientAdmin.Services.LocalStorage.ILocalStorageService;

#nullable disable

namespace ClientAdmin.Services.Api.Drivers
{
    public class DriverService : IDriverService
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILocalStorageService _localStorageService;

        private readonly HttpClient _httpClient;

        public DriverService(IAuthenticationService authenticationService, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _authenticationService = authenticationService;

            _localStorageService = localStorageService;

            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<Response<List<Driver>>> GetAsync()
        {
            ApiHandler<List<Driver>> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a GET request to the API to get drivers
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/admins/me/drivers/");

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Driver>> CreateAsync(RegisterDriverRequest driver)
        {
            ApiHandler<Driver> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a POST request to the API to create a driver
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/admins/me/drivers/");
            httpRequest.Content = driver.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Driver>> UpdateAsync(RegisterDriverRequest driver, int id)
        {
            ApiHandler<Driver> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a POST request to the API to create a driver
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/admins/me/drivers/{id}");
            httpRequest.Content = driver.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }
    }
}
