using ClientAdmin.Handlers;
using ClientAdmin.Services.Authentication;
using Models;
using Models.Resources;
using ILocalStorageService = ClientAdmin.Services.LocalStorage.ILocalStorageService;

#nullable disable

namespace ClientAdmin.Services.Api.Stores
{
    public class StoreService : IStoreService
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILocalStorageService _localStorageService;

        private readonly HttpClient _httpClient;

        public StoreService(IAuthenticationService authenticationService, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _authenticationService = authenticationService;

            _localStorageService = localStorageService;

            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<Response<Store>> GetAsync()
        {
            ApiHandler<Store> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a GET request to the API to get a store
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/admins/me/stores/");

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Store>> CreateAsync(Store store)
        {
            ApiHandler<Store> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a POST request to the API to create a store
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/admins/me/stores/");
            httpRequest.Content = store.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Store>> UpdateAsync(Store store)
        {
            ApiHandler<Store> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a PUT request to the API to update a store
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/admins/me/stores/");
            httpRequest.Content = store.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }
    }
}
