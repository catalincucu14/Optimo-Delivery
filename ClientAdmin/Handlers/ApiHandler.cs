using ClientAdmin.Services.Authentication;
using ClientAdmin.Services.LocalStorage;
using Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

#nullable disable

namespace ClientAdmin.Handlers
{
    public class ApiHandler<T>
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ILocalStorageService _localStorageService;

        private readonly HttpClient _httpClient;

        private readonly List<int> _httpStatusCodes;

        public ApiHandler(IAuthenticationService authenticationService, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            _authenticationService = authenticationService;

            _localStorageService = localStorageService;

            _httpClient = httpClient;

            _httpStatusCodes = new List<int>() { 200, 201, 400, 401, 404, 500 };
        }

        public async Task<Response<T>> HandleRequestAsync(HttpRequestMessage httpRequest)
        {
            try
            {
                // Get the JWT from local storage
                string token = await _localStorageService.ReadTokenAsync();

                // Check if the JWT is valid, if is not then logout the user
                if (string.IsNullOrEmpty(token))
                {
                    await _authenticationService.LogoutAsync();

                    return new Response<T>();
                }

                // Set the JWT to the request
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send the request
                HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest);

                // Check if the response from the API has any of the selected codes
                // If not then the action will not be performed, can't do much about it 
                if (_httpStatusCodes.Contains((int)httpResponse.StatusCode))
                {
                    // Parse the HTTP response
                    Response<T> response = await httpResponse.Content.ReadFromJsonAsync<Response<T>>();

                    if ((int)httpResponse.StatusCode == 500)
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
                    return new Response<T>
                    {
                        Success = false,
                        Errors = new List<string>() { "Action cloud not be performed" }
                    };
                }
            }
            catch
            {
                return new Response<T>
                {
                    Success = false,
                    Errors = new List<string>() { "Action cloud not be performed" }
                };
            }
        }
    }
}
