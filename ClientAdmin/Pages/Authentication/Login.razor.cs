using ClientAdmin.Services.Authentication;
using ClientAdmin.Services.Navigation;
using ClientAdmin.Utils;
using Microsoft.AspNetCore.Components;
using Models;
using Models.Authentication;

#nullable disable

namespace ClientAdmin.Pages.Authentication
{
    public partial class LoginBase : ComponentBase
    {
        [Inject]
        public INavigationService NavigationService { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        protected LoginRequest LoginRequestModel { get; set; } = new();

        protected ComponentState LoginState { get; set; } = new();

        /// <summary>
        /// Function used to submit the user credentials, if are valid authenticate the user and redirect to the Home page
        /// </summary>
        protected async Task LoginAsync()
        {
            LoginState.Errors.Clear();
            LoginState.Processing = true;

            // Authenticate the user
            Response<string> responseAuthentication = await AuthenticationService.AuthenticateAsync(LoginRequestModel);

            if (responseAuthentication.Success)
            {
                NavigationService.ToHome();
            }
            else
            {
                LoginState.Errors = responseAuthentication.Errors;
            }

            LoginState.Processing = false;
        }
    }
}
