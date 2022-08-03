using Api.Services.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Resources;
using System.Security.Claims;

#nullable disable

namespace Api.Controllers
{
    [ApiController]
    public class StoresController : Controller
    {
        private readonly DatabaseService _databaseService;

        public StoresController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Endpoint used to get the store associated with the JWT
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/stores/")]
        [HttpGet]
        public async Task<IActionResult> ReadOneByAdmin()
        {
            List<string> errors = new();

            try
            {
                // Get the admin id
                int adminId = int.Parse(User.FindFirstValue("id"));

                // Read the store
                Response<Store> responseStore = await _databaseService.Stores.ReadOneAsync(adminId);

                return StatusCode(200, responseStore);
            }
            catch (Exception e)
            {
                errors.Add(e.Message);

                return StatusCode(500, new Response<Store>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to create a store
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/stores/")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Store store)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin id
                    int adminId = int.Parse(User.FindFirstValue("id"));

                    store.AdminId = adminId;

                    // Read the store
                    Response<Store> responseStore = await _databaseService.Stores.ReadOneAsync(adminId);

                    // Check if a store is already created
                    if (responseStore.Data is not null)
                    {
                        errors.Add("STORE already created");

                        return StatusCode(400, new Response<Store>
                        {
                            Success = false,
                            Errors = errors
                        });
                    }
                    else
                    {
                        // Create the admin
                        responseStore = await _databaseService.Stores.CreateAsync(adminId, store);

                        // Check if the creation went through
                        if (!responseStore.Success)
                        {
                            return StatusCode(400, responseStore);
                        }
                        else
                        {
                            return StatusCode(201, responseStore);
                        }
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<Store>
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

                return StatusCode(400, new Response<Store>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to update a store
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/stores/")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Store store)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Get the admin id
                    int adminId = int.Parse(User.FindFirstValue("id"));

                    store.AdminId = adminId;

                    // Update the store
                    Response<Store> responseStore = await _databaseService.Stores.UpdateAsync(adminId, store);

                    // Check if the creation went through
                    if (!responseStore.Success)
                    {
                        return StatusCode(400, responseStore);
                    }
                    else
                    {
                        return StatusCode(200, responseStore);
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<Store>
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

                return StatusCode(400, new Response<Store>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }
    }
}
