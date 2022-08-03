using Api.Database;
using Api.Services.Map;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Resources;
using Models.Utils;

#nullable disable

namespace Api.Services.Database.Orders
{
    public class OrderService : IOrderService
    {
        private readonly DatabaseContext _databaseContext;

        private readonly IMapService _mapboxService;

        public OrderService(DatabaseContext databaseContext, IMapService mapboxService)
        {
            _databaseContext = databaseContext;

            _mapboxService = mapboxService;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int orderId, int adminId) => await _databaseContext.Orders
                .AnyAsync(orderTemp => orderTemp.OrderId == orderId && orderTemp.AdminId == adminId);

        /// <inheritdoc />
        public async Task<Response<List<Order>>> ReadAllAsync(int adminId, string state)
        {
            List<Order> orders = new();

            // Check if is need to filter by state
            if (string.IsNullOrEmpty(state))
            {
                orders = await _databaseContext.Orders
                    .Where(orderTemp => orderTemp.AdminId == adminId && orderTemp.RouteId == null)
                    .ToListAsync();
            }
            else
            {
                orders = await _databaseContext.Orders
                    .Where(orderTemp => orderTemp.AdminId == adminId && orderTemp.RouteId == null && orderTemp.OrderState == state)
                    .ToListAsync();
            }

            return new Response<List<Order>>
            {
                Success = true,
                Data = orders
            };
        }

        /// <inheritdoc />
        public async Task<Response<Order>> CreateAsync(int adminId, Order order)
        {
            List<string> errors = new();

            // Check if the custom id is not used by another order from the same admin
            if (await _databaseContext.Orders.AnyAsync(orderTemp => orderTemp.CustomId == order.CustomId && orderTemp.AdminId == adminId) && !string.IsNullOrEmpty(order.CustomId))
            {
                errors.Add("CUSTOM ID duplicate");

                return new Response<Order>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Get the country of the admin
                string country = (await _databaseContext.Admins
                    .Where(adminTemp => adminTemp.AdminId == adminId)
                    .FirstAsync())
                    .Country;

                // Get the coordinates of the location from Mapbox API
                Coordinates coordinates = _mapboxService.GetCoordinates(country, order.City, order.Address);

                // Set the coordinates to the store
                order.Latitude = coordinates.Latitude;
                order.Longitude = coordinates.Longitude;

                // Create the order
                await _databaseContext.Orders.AddAsync(order);

                // Save changes
                await _databaseContext.SaveChangesAsync();

                return new Response<Order>
                {
                    Success = true,
                    Data = order
                };
            }
        }

        /// <inheritdoc />
        public async Task<Response<Order>> UpdateAsync(int orderId, int adminId, Order order)
        {
            List<string> errors = new();

            // Read the order
            Order orderVar = await _databaseContext.Orders
                .Where(orderTemp => orderTemp.AdminId == adminId && orderTemp.OrderId == orderId)
                .FirstAsync();

            // Check if the custom id is not used by another order from the same admin
            if (await _databaseContext.Orders.AnyAsync(orderTemp => orderTemp.CustomId == order.CustomId && orderTemp.OrderId != order.OrderId && orderTemp.AdminId == adminId) && !string.IsNullOrEmpty(order.CustomId))
            {
                errors.Add("CUSTOM ID duplicate");

                return new Response<Order>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Update the coodinates only if the city or address have been changed
                if (orderVar.City != order.City && orderVar.Address != order.Address)
                {
                    // Get the country of the admin
                    string country = (await _databaseContext.Admins
                        .Where(adminTemp => adminTemp.AdminId == adminId)
                        .FirstAsync())
                        .Country;

                    // Get the coordinates of the location from Mapbox API
                    Coordinates coordinates = _mapboxService.GetCoordinates(country, order.City, order.Address);

                    // Set the coordinates to the store
                    order.Latitude = coordinates.Latitude;
                    order.Longitude = coordinates.Longitude;
                }

                // Update the order
                orderVar.CustomId = order.CustomId;
                orderVar.FirstName = order.FirstName;
                orderVar.LastName = order.LastName;
                orderVar.Mail = order.Mail;
                orderVar.Phone = order.Phone;
                orderVar.LeftToPay = order.LeftToPay;
                orderVar.City = order.City;
                orderVar.Address = order.Address;
                orderVar.OrderState = order.OrderState;

                // Save changes
                await _databaseContext.SaveChangesAsync();

                return new Response<Order>
                {
                    Success = true,
                    Data = order
                };
            }
        }

        /// <inheritdoc />
        public async Task<Response<Order>> DeleteAsync(int orderId, int adminId)
        {
            List<string> errors = new();

            // Read the order
            Order orderVar = await _databaseContext.Orders
                .Where(orderTemp => orderTemp.AdminId == adminId && orderTemp.OrderId == orderId)
                .FirstAsync(); ;

            if (orderVar.RouteId == 0)
            {
                errors.Add("ORDER is part of a route");

                return new Response<Order>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Delete the order
                _databaseContext.Orders.Remove(orderVar);

                // Save changes
                await _databaseContext.SaveChangesAsync();

                return new Response<Order>
                {
                    Success = true
                };
            }
        }
    }
}
