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
    public class OrdersController : Controller
    {
        private readonly DatabaseService _databaseService;

        public OrdersController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Endpoint used to get the orders associated with the JWT that are not assigned to a route
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/orders/")]
        [HttpGet]
        public async Task<IActionResult> ReadAllAsync(string state = "")
        {
            List<string> errors = new();

            try
            {
                // Get the admin id
                int adminId = int.Parse(User.FindFirstValue("id"));

                // Read the orders
                Response<List<Order>> responseOrders = await _databaseService.Orders.ReadAllAsync(adminId, state);

                return StatusCode(200, responseOrders);
            }
            catch (Exception e)
            {
                errors.Add(e.Message);

                return StatusCode(500, new Response<List<Order>>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to create an order
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/orders/")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Order order)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin id
                    int adminId = int.Parse(User.FindFirstValue("id"));

                    order.AdminId = adminId;

                    // Create the order
                    Response<Order> responseOrder = await _databaseService.Orders.CreateAsync(adminId, order);

                    // Check if the action went through
                    if (!responseOrder.Success)
                    {
                        return StatusCode(400, responseOrder);
                    }
                    else
                    {
                        return StatusCode(201, responseOrder);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<Order>
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

                return StatusCode(400, new Response<Order>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to update an order
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/orders/{orderId:int}/")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] Order order, int orderId)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin id
                    int adminId = int.Parse(User.FindFirstValue("id"));

                    order.AdminId = adminId;

                    // Check if the requested order exists
                    if (!await _databaseService.Orders.ExistsAsync(orderId, adminId))
                    {
                        errors.Add("ORDER not found");

                        return StatusCode(404, new Response<Order>
                        {
                            Success = false,
                            Errors = errors
                        });
                    }
                    else
                    {
                        // Update the order
                        Response<Order> responseOrder = await _databaseService.Orders.UpdateAsync(orderId, adminId, order);

                        // Check if the action went through
                        if (!responseOrder.Success)
                        {
                            return StatusCode(400, responseOrder);
                        }
                        else
                        {
                            return StatusCode(200, responseOrder);
                        }
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<Order>
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

                return StatusCode(400, new Response<Order>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to delete an order 
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/orders/{orderId:int}/")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int orderId)
        {
            List<string> errors = new();

            try
            {
                // Get the admin id
                int adminId = int.Parse(User.FindFirstValue("id"));

                // Check if the requested order exists
                if (!await _databaseService.Orders.ExistsAsync(orderId, adminId))
                {
                    errors.Add("ORDER not found");

                    return StatusCode(404, new Response<Order>
                    {
                        Success = false,
                        Errors = errors
                    });
                }
                else
                {
                    // Update the order
                    Response<Order> response = await _databaseService.Orders.DeleteAsync(orderId, adminId);

                    // Check if the action went through
                    if (!response.Success)
                    {
                        return StatusCode(400, response);
                    }
                    else
                    {
                        return StatusCode(200, response);
                    }
                }
            }
            catch (Exception e)
            {
                errors.Add(e.Message);

                return StatusCode(500, new Response<Order>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }
    }
}
