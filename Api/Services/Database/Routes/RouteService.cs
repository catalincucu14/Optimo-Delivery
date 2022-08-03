using Api.Database;
using Microsoft.EntityFrameworkCore;
using Models;
using Route = Models.Resources.Route;

#nullable disable

namespace Api.Services.Database.Routes
{
    public class RouteService : IRouteService
    {
        private readonly DatabaseContext _databaseContext;

        public RouteService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int routeId, int adminId) => await _databaseContext.Routes
                .AnyAsync(routeTemp => routeTemp.RouteId == routeId && routeTemp.AdminId == adminId);

        /// <inheritdoc />
        public async Task<Response<List<Route>>> ReadAllAsync(int adminId)
        {
            List<string> errors = new();
            List<Route> routes = new();

            // Read the routes
            routes = await _databaseContext.Routes
                .Where(routeTemp => routeTemp.AdminId == adminId)
                .Include(routeTemp => routeTemp.Orders)
                .ToListAsync();

            return new Response<List<Route>>
            {
                Success = true,
                Data = routes
            };
        }

        /// <inheritdoc />
        public async Task<Response<Route>> CreateAsync(int adminId, Route route)
        {
            List<string> errors = new();

            // Check if the name is not used by another route from the same admin
            if (await _databaseContext.Routes.AnyAsync(routeTemp => routeTemp.Name == route.Name && routeTemp.AdminId == adminId))
            {
                errors.Add("NAME duplicate");

                return new Response<Route>
                {
                    Success = false,
                    Errors = errors
                };
            }
            // Check for duplicate order ids 
            else if (route.Orders.Count != route.Orders.GroupBy(order => order.OrderId).ToList().Count)
            {
                errors.Add("ORDERS duplicate");

                return new Response<Route>
                {
                    Success = false,
                    Errors = errors
                };
            }
            // Check if the route has at least 5 routes given
            else if (route.Orders is null || route.Orders.GroupBy(order => order.OrderId).ToList().Count < 5)
            {
                errors.Add("A route should have at least 5 orders assigned");

                return new Response<Route>
                {
                    Success = false,
                    Errors = errors
                };
            }
            // Check if the route has at most 24 routes given
            else if (route.Orders is null || route.Orders.GroupBy(order => order.OrderId).ToList().Count > 24)
            {
                errors.Add("A route should have at most 24 orders assigned");

                return new Response<Route>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Check if all given routes are not part of another route or are delivered
                route.Orders.ForEach(orderTemp =>
                {
                    // Check if the order is not assigned to another route
                    if (orderTemp.RouteId != null)
                    {
                        errors.Add($"ORDER [ {orderTemp.OrderId} ] is part of another route");
                    }
                    // Check if the order is undelivered
                    if (orderTemp.OrderState != "UNDELIVERED")
                    {
                        errors.Add($"RODER [ {orderTemp.OrderId} ] is already DELIVERED or CANCELLED");
                    }
                });

                // Check if any of the above checks have triggered
                if (errors.Count > 0)
                {
                    return new Response<Route>
                    {
                        Success = false,
                        Errors = errors
                    };
                }
                else
                {
                    // Get the ids of the assigned orders
                    List<int> ids = route.Orders
                        .Select(orderTemp => orderTemp.OrderId)
                        .ToList();

                    // Add the assigned orders 
                    route.Orders = await _databaseContext.Orders
                        .Where(orderTemp => ids.Contains(orderTemp.OrderId) && orderTemp.AdminId == adminId)
                        .ToListAsync();

                    // Create the route
                    await _databaseContext.Routes.AddAsync(route);

                    // Save changes
                    await _databaseContext.SaveChangesAsync();

                    return new Response<Route>
                    {
                        Success = true,
                        Data = route
                    };
                }
            }
        }

        /// <inheritdoc />
        public async Task<Response<Route>> UpdateAsync(int routeId, int adminId, Route route)
        {
            List<string> errors = new();

            Route routeVar = await _databaseContext.Routes
                .Where(routeTemp => routeTemp.RouteId == routeId && routeTemp.AdminId == adminId)
                .Include(routeTemp => routeTemp.Orders)
                .FirstAsync();

            // Check if the name is not used by another route from the same admin
            if (await _databaseContext.Routes.AnyAsync(routeTemp => routeTemp.Name == route.Name && routeTemp.RouteId != routeId && routeTemp.AdminId == adminId))
            {
                errors.Add("NAME duplicate");

                return new Response<Route>
                {
                    Success = false,
                    Errors = errors
                };
            }
            // Check for duplicate orders
            else if (route.Orders.Count != route.Orders.GroupBy(order => order.OrderId).ToList().Count)
            {
                errors.Add("ORDERS duplicate");

                return new Response<Route>
                {
                    Success = false,
                    Errors = errors
                };
            }
            // Check if the route has at least 5 routes given
            else if (route.Orders is null || route.Orders.GroupBy(order => order.OrderId).ToList().Count < 5)
            {
                errors.Add("A route should have at least 5 orders assigned");

                return new Response<Route>
                {
                    Success = false,
                    Errors = errors
                };
            }
            // Check if the route has at most 24 routes given
            else if (route.Orders is null || route.Orders.GroupBy(order => order.OrderId).ToList().Count > 24)
            {
                errors.Add("A route should have at most 24 orders assigned");

                return new Response<Route>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Check if all given routes are not part of another route or are delivered
                route.Orders.ForEach(orderTemp =>
                {
                    // Check if the order is not already assigned to the route
                    // If yes then is no need to update it
                    if (orderTemp.RouteId == null || orderTemp.RouteId == routeId)
                    {
                        return;
                    }
                    // Check if the order is not assigned to another route
                    if (orderTemp.RouteId != null)
                    {
                        errors.Add($"ORDER [ {orderTemp.OrderId} ] is part of another route");
                    }
                    // Check if the order is undelivered
                    if (orderTemp.OrderState != "UNDELIVERED")
                    {
                        errors.Add($"RODER [ {orderTemp.OrderId} ] is already DELIVERED or CANCELLED");
                    }
                });

                // Check if any of the above checks have triggered
                if (errors.Count > 0)
                {
                    return new Response<Route>
                    {
                        Success = false,
                        Errors = errors
                    };
                }
                else
                {
                    // Get the ids of the assigned orders
                    List<int> ids = route.Orders
                        .Select(orderTemp => orderTemp.OrderId)
                        .ToList();

                    // Remove the unassigned orders
                    routeVar.Orders
                        .Where(orderTemp => !ids.Contains(orderTemp.OrderId))
                        .ToList()
                        .ForEach(orderTemp => routeVar.Orders.Remove(orderTemp));

                    // Add the assigned orders 
                    routeVar.Orders = await _databaseContext.Orders
                        .Where(orderTemp => ids.Contains(orderTemp.OrderId) && orderTemp.AdminId == adminId)
                        .ToListAsync();

                    // Update the route
                    routeVar.DriverId = route.DriverId;
                    routeVar.Name = route.Name;
                    routeVar.Completed = route.Completed;

                    // Save changes
                    await _databaseContext.SaveChangesAsync();

                    return new Response<Route>
                    {
                        Success = true,
                        Data = routeVar
                    };
                }
            }
        }

        /// <inheritdoc />
        public async Task<Response<Route>> DeleteAsync(int routeId, int adminId)
        {
            // Read the assigned orders of the route
            Route routeVar = await _databaseContext.Routes
                .Where(routeTemp => routeTemp.RouteId == routeId && routeTemp.AdminId == adminId)
                .Include(routeTemp => routeTemp.Orders)
                .FirstAsync();

            // Delete the order
            _databaseContext.Routes.Remove(routeVar);

            // Save changes
            await _databaseContext.SaveChangesAsync();

            return new Response<Route>
            {
                Success = true
            };
        }
    }
}
