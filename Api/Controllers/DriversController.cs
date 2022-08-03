using Api.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Authentication;
using Models.Resources;
using System.Security.Claims;

#nullable disable

namespace Api.Controllers
{
    [ApiController]
    public class DriversController : Controller
    {
        private readonly DatabaseService _databaseService;

        public DriversController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Endpoint used to get the drivers associated with the JWT
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/drivers/")]
        [HttpGet]
        public async Task<IActionResult> ReadAllAsync()
        {
            List<string> errors = new();

            try
            {
                // Get the admin id
                int adminId = int.Parse(User.FindFirstValue("id"));

                // Read the drivers
                Response<List<Driver>> responseDrivers = await _databaseService.Drivers.ReadAllAsync(adminId);

                return StatusCode(200, responseDrivers);
            }
            catch (Exception e)
            {
                errors.Add(e.Message);

                return StatusCode(500, new Response<List<Driver>>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to create a driver
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/drivers/")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RegisterDriverRequest registerRequest)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin id
                    int adminId = int.Parse(User.FindFirstValue("id"));

                    // Create the driver
                    Response<Driver> responseDriver = await _databaseService.Drivers.CreateAsync(adminId, registerRequest);

                    // Check if the action went through
                    if (!responseDriver.Success)
                    {
                        return StatusCode(400, responseDriver);
                    }
                    else
                    {
                        return StatusCode(201, responseDriver);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<Driver>
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

                return StatusCode(400, new Response<Driver>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to update a driver 
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/drivers/{driverId:int}/")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] RegisterDriverRequest registerRequest, int driverId)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin id
                    int adminId = int.Parse(User.FindFirstValue("id"));

                    // Check if the requested driver exists
                    if (!await _databaseService.Drivers.ExistsAsync(driverId, adminId))
                    {
                        errors.Add("DRIVER not found");

                        return StatusCode(404, new Response<Driver>
                        {
                            Success = false,
                            Errors = errors
                        });
                    }
                    else
                    {
                        // Update the driver
                        Response<Driver> responseDriver = await _databaseService.Drivers.UpdateAsync(driverId, adminId, registerRequest);

                        // Check if the action went through
                        if (!responseDriver.Success)
                        {
                            return StatusCode(400, responseDriver);
                        }
                        else
                        {
                            return StatusCode(200, responseDriver);
                        }
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<Driver>
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

                return StatusCode(400, new Response<Driver>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }
    }
}
