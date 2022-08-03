using ClientAdmin;
using ClientAdmin.Handlers;
using ClientAdmin.Services.Api;
using ClientAdmin.Services.Authentication;
using ClientAdmin.Services.Navigation;
using ClientAdmin.Services.RoutePlanner;
using Blazored.LocalStorage;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using ILocalStorageService = ClientAdmin.Services.LocalStorage.ILocalStorageService;
using LocalStorageService = ClientAdmin.Services.LocalStorage.LocalStorageService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.SetMinimumLevel(LogLevel.Error);

builder.Services
    .AddScoped(serviceProvider => new HttpClient
    {
        BaseAddress = new Uri("https://localhost:44338"),
    });

builder.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

builder.Services.AddAuthorizationCore();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddBlazoredModal();

builder.Services.AddTransient<AuthenticationStateProvider, AuthenticationHandler>();

builder.Services.AddTransient<ApiService>();

builder.Services.AddTransient<ILocalStorageService, LocalStorageService>();

builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

builder.Services.AddTransient<INavigationService, NavigationService>();

builder.Services.AddTransient<IRoutePlannerService, RoutePlannerService>();

await builder.Build().RunAsync();
