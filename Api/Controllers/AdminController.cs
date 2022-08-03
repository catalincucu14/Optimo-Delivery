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
    public class AdminController : Controller
    {
        private readonly DatabaseService _databaseService;

        public AdminController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Endpoint used to get the admin associated with the JWT
        /// </summary>
        [Authorize(Roles = "Admin")]
        [Route("api/admins/me/")]
        [HttpGet]
        public async Task<IActionResult> ReadOneAsync()
        {
            List<string> errors = new();

            try
            {
                // Get the admin id
                int adminId = int.Parse(User.FindFirstValue("id"));

                // Check if the requested admin exists
                if (!await _databaseService.Admins.ExistsAsync(adminId))
                {
                    errors.Add("ADMIN not found");

                    return StatusCode(404, new Response<Admin>
                    {
                        Success = false,
                        Errors = errors
                    });
                }
                else
                {
                    // Read the admin
                    Response<Admin> responseAdmin = await _databaseService.Admins.ReadOneAsync(adminId);

                    return StatusCode(200, responseAdmin);
                }
            }
            catch (Exception e)
            {
                errors.Add(e.Message);

                return StatusCode(500, new Response<Admin>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }
    }
}
