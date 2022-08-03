using ClientAdmin.Services.LocalStorage;
using ClientAdmin.Services.Navigation;
using Models;
using Models.Authentication;
using System.Net.Http.Json;

#nullable disable

namespace ClientAdmin.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly INavigationService _navigationService;

        private readonly ILocalStorageService _localStorageService;

        private readonly HttpClient _httpClient;

        private readonly List<int> _httpStatusCodes;

        public AuthenticationService(INavigationService navigationService, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _localStorageService = localStorageService;

            _navigationService = navigationService;

            _httpClient = httpClient;

            _httpStatusCodes = new List<int>() { 200, 201, 400, 401, 404, 500 };
        }

        /// <inheritdoc />
        public async Task<Response<string>> AuthenticateAsync<T>(T authenticationRequest)
        {
            HttpResponseMessage httpResponse;

            try
            {
                // Create a POST request to the API with the login/register credentials
                if (authenticationRequest is LoginRequest)
                {
                    httpResponse = await _httpClient.PostAsJsonAsync<T>("/api/authentication/login/admins/", authenticationRequest);
                }
                else
                {
                    httpResponse = await _httpClient.PostAsJsonAsync<T>("/api/authentication/register/admins/", authenticationRequest);
                }

                // Check if the response from the API has any of the selected codes
                // If not then the action will not be performed, can't do much about it 
                if (_httpStatusCodes.Contains((int)httpResponse.StatusCode))
                {
                    // Parse the HTTP response
                    Response<string> response = await httpResponse.Content.ReadFromJsonAsync<Response<string>>();

                    if (response.Success)
                    {
                        // Set the JWT in local storage
                        await _localStorageService.WriteTokenAsync(response.Data);
                    }
                    else if ((int)httpResponse.StatusCode == 500)
                    {
                        foreach (string error in response.Errors)
                        {
                            Console.WriteLine(error);
                        }

                        response.Errors = new List<string> { "Internal server error" };
                    }

                    return response;
                }
                else
                {
                    return new Response<string>
                    {
                        Success = false,
                        Errors = new List<string>() { "Action cloud not be performed" }
                    };
                }
            }
            catch
            {
                return new Response<string>
                {
                    Success = false,
                    Errors = new List<string>() { "Action cloud not be performed" }
                };
            }
        }

        /// <inheritdoc />
        public async Task LogoutAsync()
        {
            // Remove the JTW from local storage
            await _localStorageService.RemoveTokenAsync();

            // Redirect to Login page
            _navigationService.ToLogin();
        }
    }
}
