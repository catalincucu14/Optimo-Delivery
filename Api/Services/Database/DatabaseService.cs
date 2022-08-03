using Api.Database;
using Api.Services.Database.Admins;
using Api.Services.Database.Drivers;
using Api.Services.Database.Orders;
using Api.Services.Database.Routes;
using Api.Services.Database.Stores;
using Api.Services.Map;

namespace Api.Services.Database
{
    public class DatabaseService
    {
        public readonly IAdminService Admins;

        public readonly IDriverService Drivers;

        public readonly IOrderService Orders;

        public readonly IRouteService Routes;

        public readonly IStoreService Stores;

        public DatabaseService(DatabaseContext databaseContext, IMapService mapboxService)
        {
            Admins = new AdminService(databaseContext);

            Drivers = new DriverService(databaseContext);

            Routes = new RouteService(databaseContext);

            Orders = new OrderService(databaseContext, mapboxService);

            Stores = new StoreService(databaseContext, mapboxService);
        }
    }
}
