using ClientAdmin.Handlers;
using ClientAdmin.Services.Authentication;
using Models;
using Models.Resources;
using ILocalStorageService = ClientAdmin.Services.LocalStorage.ILocalStorageService;

#nullable disable

namespace ClientAdmin.Services.Api.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILocalStorageService _localStorageService;

        private readonly HttpClient _httpClient;

        public OrderService(IAuthenticationService authenticationService, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _authenticationService = authenticationService;

            _localStorageService = localStorageService;

            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<Response<List<Order>>> GetAsync(string state)
        {
            ApiHandler<List<Order>> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a GET request to the API to get orders
            HttpRequestMessage httpRequest = string.IsNullOrEmpty(state) ? new HttpRequestMessage(HttpMethod.Get, $"/api/admins/me/orders/") : new HttpRequestMessage(HttpMethod.Get, $"/api/admins/me/orders?state={state}");

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Order>> CreateAsync(Order order)
        {
            ApiHandler<Order> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a POST request to the API to create an order
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/admins/me/orders/");
            httpRequest.Content = order.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Order>> UpdateAsync(Order order)
        {
            ApiHandler<Order> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a PUT request to the API to update an order
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/admins/me/orders/{order.OrderId}");
            httpRequest.Content = order.Json();

            return await apiHandler.HandleRequestAsync(httpRequest);
        }

        /// <inheritdoc />
        public async Task<Response<Order>> DeleteAsync(Order order)
        {
            ApiHandler<Order> apiHandler = new(_authenticationService, _localStorageService, _httpClient);

            // Create a DELETE request to the API to delete an order
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/admins/me/orders/{order.OrderId}");

            return await apiHandler.HandleRequestAsync(httpRequest);
        }
    }
}
