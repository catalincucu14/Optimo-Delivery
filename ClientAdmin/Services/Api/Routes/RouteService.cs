using ClientAdmin.Handlers;
using ClientAdmin.Services.Authentication;
using Models;
using Models.Resources;
using ILocalStorageService = ClientAdmin.Services.LocalStorage.ILocalStorageService;

#nullable disable

namespace ClientAdmin.Services.Api.Routes
{
    public class RouteService : IRouteService
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILocalStorageService _localStorageService;

        private readonly HttpClient _httpClient;

        public RouteService(IAuthenticationService authenticationService, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _authenticationService = authenticationService;

            _localStorageService = localStorageService;

            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<Response<List<Route>>> GetAsync()
        {
            ApiHandler<List<Route>> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a GET request to the API to get routes
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/admins/me/routes/");

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Route>> CreateAsync(Route route)
        {
            ApiHandler<Route> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a POST request to the API to create a route
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/admins/me/routes/");
            httpRequest.Content = route.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Route>> UpdateAsync(Route route)
        {
            ApiHandler<Route> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a PUT request to the API to update a route
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/admins/me/routes/{route.RouteId}");
            httpRequest.Content = route.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Route>> DeleteAsync(Route route)
        {
            ApiHandler<Route> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a DELETE request to the API to delete a route
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/admins/me/routes/{route.RouteId}");
            httpRequest.Content = route.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }
    }
}
