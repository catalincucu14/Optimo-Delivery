using ClientAdmin.Services.Api.Admins;
using ClientAdmin.Services.Api.Drivers;
using ClientAdmin.Services.Api.Orders;
using ClientAdmin.Services.Api.Routes;
using ClientAdmin.Services.Api.Stores;
using ClientAdmin.Services.Authentication;
using ClientAdmin.Services.LocalStorage;

namespace ClientAdmin.Services.Api
{
    public class ApiService
    {
        public readonly IAdminService Admins;

        public readonly IDriverService Drivers;

        public readonly IOrderService Orders;

        public readonly IRouteService Routes;

        public readonly IStoreService Stores;

        public ApiService(IAuthenticationService authenticationService, ILocalStorageService localStorageService, HttpClient httpClient)
        {
            Admins = new AdminService(authenticationService, localStorageService, httpClient);

            Drivers = new DriverService(authenticationService, localStorageService, httpClient);

            Routes = new RouteService(authenticationService, localStorageService, httpClient);

            Orders = new OrderService(authenticationService, localStorageService, httpClient);

            Stores = new StoreService(authenticationService, localStorageService, httpClient);
        }
    }
}
