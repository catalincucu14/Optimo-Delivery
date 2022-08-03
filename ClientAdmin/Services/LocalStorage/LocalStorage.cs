using ClientAdmin.Handlers;
using Microsoft.AspNetCore.Components.Authorization;

#nullable disable

namespace ClientAdmin.Services.LocalStorage
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly AuthenticationStateProvider _authenticationHandler;

        private readonly Blazored.LocalStorage.ILocalStorageService _localStorageService;

        public LocalStorageService(AuthenticationStateProvider authenticationHandler, Blazored.LocalStorage.ILocalStorageService localStorageService)
        {
            _authenticationHandler = authenticationHandler;

            _localStorageService = localStorageService;
        }

        /// <inheritdoc />
        public async Task WriteTokenAsync(dynamic token)
        {
            // Set the JWT in local storage
            await _localStorageService.SetItemAsync("token", token);

            // Raise the authentication event
            (_authenticationHandler as AuthenticationHandler).RaiseAuthenticationStateChanged();
        }

        /// <inheritdoc />
        public async Task RemoveTokenAsync()
        {
            // Remove the JWT token in local storage
            await _localStorageService.RemoveItemAsync("token");

            // Raise the authentication event
            (_authenticationHandler as AuthenticationHandler).RaiseAuthenticationStateChanged();
        }

        /// <inheritdoc />
        public async Task<string> ReadTokenAsync()
        {
            // Get the JWT from local storage
            string token = await _localStorageService.GetItemAsync<string>("token");

            // Check if the JWT is not empty or expired
            if (string.IsNullOrEmpty(token) || JwtHandler.IsExpired(JwtHandler.Decode(token)))
            {
                return null;
            }
            else
            {
                return token;
            }
        }
    }
}
