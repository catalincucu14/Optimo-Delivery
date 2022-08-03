using ClientAdmin.Services.Authentication;
using ClientAdmin.Services.Navigation;
using ClientAdmin.Utils;
using Microsoft.AspNetCore.Components;
using Models;
using Models.Authentication;

#nullable disable

namespace ClientAdmin.Pages.Authentication
{
    public partial class RegisterBase : ComponentBase
    {
        [Inject]
        public INavigationService NavigationService { get; set; }

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        protected RegisterAdminRequest RegisterRequestModel { get; set; } = new();

        protected ComponentState RegisterState { get; set; } = new();

        /// <summary>
        /// Function used to submit the user credentials, if are valid create and then authenticate the user and redirect to the Home page
        /// </summary>
        protected async Task RegisterAsync()
        {
            RegisterState.Errors.Clear();
            RegisterState.Processing = true;

            // Authenticate the user
            Response<string> responseAuthentication = await AuthenticationService.AuthenticateAsync(RegisterRequestModel);

            if (responseAuthentication.Success)
            {
                NavigationService.ToHome();
            }
            else
            {
                RegisterState.Errors = responseAuthentication.Errors;
            }

            RegisterState.Processing = false;
        }
    }
}
