using Api.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Resources;
using System.Security.Claims;
using Route = Models.Resources.Route;

#nullable disable

namespace Api.Controllers
{
    [ApiController]
    public class RoutesController : Controller
    {
        private readonly DatabaseService _databaseService;

        public RoutesController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Endpoint used to get the routes associated with the JWT
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/routes/")]
        [HttpGet]
        public async Task<IActionResult> ReadAllAsync()
        {
            List<string> errors = new();

            try
            {
                // Get the admin id
                int adminId = int.Parse(User.FindFirstValue("id"));

                // Read the routes
                Response<List<Route>> response = await _databaseService.Routes.ReadAllAsync(adminId);

                return StatusCode(200, response);
            }
            catch (Exception e)
            {
                errors.Add(e.Message);

                return StatusCode(500, new Response<List<Route>>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to create a route
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/routes/")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Route route)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin id
                    int adminId = int.Parse(User.FindFirstValue("id"));

                    route.AdminId = adminId;

                    // Read the store 
                    Response<Store> responseStore = await _databaseService.Stores.ReadOneAsync(adminId);

                    // Check if the store is created/configured
                    // To create a route the store must be created/configured
                    if (responseStore.Data is null)
                    {
                        errors.Add("STORE not found");
                    }

                    // Check if the requested driver exists
                    if (!await _databaseService.Drivers.ExistsAsync(route.DriverId, adminId))
                    {
                        errors.Add("DRIVER not found");
                    }

                    // Check if the requested orders exist
                    foreach (Order order in route.Orders)
                    {
                        if (!await _databaseService.Orders.ExistsAsync(order.OrderId, adminId))
                        {
                            errors.Add($"ORDER [ {order.OrderId} ] not found");
                        }
                    }

                    // Check if any of the above checks have triggered
                    if (errors.Count > 0)
                    {
                        return StatusCode(404, new Response<Route>
                        {
                            Success = false,
                            Errors = errors
                        });
                    }

                    // Create the route
                    Response<Route> responseRoute = await _databaseService.Routes.CreateAsync(adminId, route);

                    // Check if the action went through
                    if (!responseRoute.Success)
                    {
                        return StatusCode(400, responseRoute);
                    }
                    else
                    {
                        return StatusCode(201, responseRoute);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<Route>
                    {
                        Success = false,
                        Errors = errors
                    });
                }
            }
            else
            {
                errors = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return StatusCode(400, new Response<Route>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to update a route
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/routes/{routeId:int}/")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Route route, int routeId)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin id
                    int adminId = int.Parse(User.FindFirstValue("id"));

                    route.AdminId = adminId;

                    // Read the store 
                    Response<Store> responseStore = await _databaseService.Stores.ReadOneAsync(adminId);

                    // Check if the store is created/configured
                    // To create a route the store must be created/configured
                    if (responseStore.Data is null)
                    {
                        errors.Add("STORE not found");
                    }

                    // Check if the requested driver exists
                    if (!await _databaseService.Drivers.ExistsAsync(route.DriverId, adminId))
                    {
                        errors.Add("DRIVER not found");
                    }

                    // Check if the requested route exists
                    if (!await _databaseService.Routes.ExistsAsync(routeId, adminId))
                    {
                        errors.Add("ROUTE not found");
                    }

                    // Check if the requested orders exist
                    foreach (Order order in route.Orders)
                    {
                        if (!await _databaseService.Orders.ExistsAsync(order.OrderId, adminId))
                        {
                            errors.Add($"ORDER [ {order.OrderId} ] not found");
                        }
                    }

                    // Check if any of the above checks have triggered
                    if (errors.Count > 0)
                    {
                        return StatusCode(404, new Response<Route>
                        {
                            Success = false,
                            Errors = errors
                        });
                    }

                    // Create the route
                    Response<Route> responseRoute = await _databaseService.Routes.UpdateAsync(routeId, adminId, route);

                    // Check if the action went through
                    if (!responseRoute.Success)
                    {
                        return StatusCode(400, responseRoute);
                    }
                    else
                    {
                        return StatusCode(200, responseRoute);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<Route>
                    {
                        Success = false,
                        Errors = errors
                    });
                }
            }
            else
            {
                errors = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return StatusCode(400, new Response<Route>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to delete a route 
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/routes/{routeId:int}/")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int routeId)
        {
            List<string> errors = new();

            try
            {
                // Get the admin id
                int adminId = int.Parse(User.FindFirstValue("id"));

                // Check if the requested route exists
                if (!await _databaseService.Routes.ExistsAsync(routeId, adminId))
                {
                    errors.Add("ROUTE not found");

                    return StatusCode(404, new Response<Route>
                    {
                        Success = false,
                        Errors = errors
                    });
                }
                else
                {
                    Response<Route> responseRoute = await _databaseService.Routes.DeleteAsync(routeId, adminId);

                    // Check if the action went through
                    if (!responseRoute.Success)
                    {
                        return StatusCode(400, responseRoute);
                    }
                    else
                    {
                        return StatusCode(200, responseRoute);
                    }
                }
            }
            catch (Exception e)
            {
                errors.Add(e.Message);

                return StatusCode(500, new Response<Route>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }
    }
}
