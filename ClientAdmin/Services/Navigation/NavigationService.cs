using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ClientAdmin.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly NavigationManager _navigationManager;

        private readonly IJSRuntime _jsRuntime;

        public NavigationService(NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            _navigationManager = navigationManager;

            _jsRuntime = jsRuntime;
        }

        /// <inheritdoc />
        public void ToHome() => _navigationManager.NavigateTo("/");

        /// <inheritdoc />
        public void ToServerError() => _navigationManager.NavigateTo("/server-error");

        /// <inheritdoc />
        public void ToHelp() => _navigationManager.NavigateTo("/help");

        /// <inheritdoc />
        public void ToLogin() => _navigationManager.NavigateTo("/authentication/login");

        /// <inheritdoc />
        public void ToRegister() => _navigationManager.NavigateTo("/authentication/register");

        /// <inheritdoc />
        public void ToOrders() => _navigationManager.NavigateTo("/account/orders");

        /// <inheritdoc />
        public void ToRoutes() => _navigationManager.NavigateTo("/account/routes");

        /// <inheritdoc />
        public async Task ToGithubAsync() => await _jsRuntime.InvokeAsync<object>("open", "https://github.com/catalincucu14/Optimo-Delivery", "_blank");
    }
}
