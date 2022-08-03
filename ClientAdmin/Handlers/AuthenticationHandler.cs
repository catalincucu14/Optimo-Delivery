using ClientAdmin.Services.Navigation;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace ClientAdmin.Handlers
{
    public class AuthenticationHandler : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;

        private readonly INavigationService _navigationService;

        private readonly HttpClient _httpClient;

        private readonly List<int> _httpStatusCodes;

        public AuthenticationHandler(ILocalStorageService localStorageService, INavigationService navigationService, HttpClient httpClient)
        {
            _localStorageService = localStorageService;

            _navigationService = navigationService;

            _httpClient = httpClient;

            _httpStatusCodes = new List<int>() { 200, 201, 400, 401, 404, 500 };
        }

        /// <summary>
        /// Function used to get the current authentication state of the user (if is logged or not)
        /// </summary>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Get the JWT from local storage
                string? token = await _localStorageService.GetItemAsync<string>("token");

                // Check if the JWT is not empty, if it is logout the user
                if (string.IsNullOrEmpty(token))
                {
                    return await Task.FromResult(new AuthenticationState(JwtHandler.GetClaimsNotAuthorized()));
                }

                // Decode the JWT
                JwtSecurityToken decodeJwtToken = JwtHandler.Decode(token);

                // Check if the JWT is not expired
                if (JwtHandler.IsExpired(decodeJwtToken))
                {
                    return await Task.FromResult(new AuthenticationState(JwtHandler.GetClaimsNotAuthorized()));
                }

                // Check if the JWT has any claims
                if (JwtHandler.HasNoClaims(decodeJwtToken))
                {
                    return await Task.FromResult(new AuthenticationState(JwtHandler.GetClaimsNotAuthorized()));
                }

                // Create a GET request to the API to check if the admin exists
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/admins/me");
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send the request
                HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest);

                // Check if the response from the API has any of the selected codes
                // If not then the action will not be performed, can't do much about it 
                if (_httpStatusCodes.Contains((int)httpResponse.StatusCode))
                {
                    if ((int)httpResponse.StatusCode == 200)
                    {
                        return await Task.FromResult(new AuthenticationState(JwtHandler.GetClaimsAuthorized(decodeJwtToken)));
                    }
                    else if ((int)httpResponse.StatusCode == 500)
                    {
                        _navigationService.ToServerError();

                        return await Task.FromResult(new AuthenticationState(JwtHandler.GetClaimsNotAuthorized()));
                    }
                    else
                    {
                        return await Task.FromResult(new AuthenticationState(JwtHandler.GetClaimsNotAuthorized()));
                    }
                }
                else
                {
                    return await Task.FromResult(new AuthenticationState(JwtHandler.GetClaimsNotAuthorized()));
                }
            }
            catch
            {
                _navigationService.ToServerError();

                return await Task.FromResult(new AuthenticationState(JwtHandler.GetClaimsNotAuthorized()));
            }
        }

        /// <summary>
        /// Function used raise the AuthenticationStateChanged event
        /// </summary>
        public void RaiseAuthenticationStateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
