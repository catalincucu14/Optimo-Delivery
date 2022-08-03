using Api.Services.Authentication;
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
    public class AuthenticationController : Controller
    {
        private readonly DatabaseService _databaseService;

        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(DatabaseService databaseService, IAuthenticationService authenticationService)
        {
            _databaseService = databaseService;

            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Endpoint used to a register an admin and get a JWT back
        /// </summary>
        [AllowAnonymous]
        [Route("api/authentication/register/admins/")]
        [HttpPost]
        public async Task<IActionResult> RegisterAdminAsync([FromBody] RegisterAdminRequest registerRequest)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Create the admin
                    Response<Admin> responseRegistration = await _databaseService.Admins.CreateAsync(registerRequest);

                    // Check if the creation went through
                    if (!responseRegistration.Success)
                    {
                        return StatusCode(400, new Response<string>
                        {
                            Success = false,
                            Errors = responseRegistration.Errors
                        });
                    }
                    else
                    {
                        // Get the claims
                        List<Claim> claims = _authenticationService.GetClaimsAdmin(responseRegistration.Data);

                        // Generate the JWT
                        string token = _authenticationService.GenerateJwtToken(claims);

                        return StatusCode(201, new Response<string>
                        {
                            Success = true,
                            Data = token
                        });
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<string>
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

                return StatusCode(400, new Response<string>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to check the credentials of an admin and get a JWT back
        /// </summary>
        [AllowAnonymous]
        [Route("api/authentication/login/admins/")]
        [HttpPost]
        public async Task<IActionResult> LoginAdminAsync([FromBody] LoginRequest loginRequest)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Authenticate the user
                    Response<Admin> responseAuthentication = await _databaseService.Admins.AuthenticateAsync(loginRequest);

                    // Check if the credentials are valid
                    if (!responseAuthentication.Success)
                    {
                        return StatusCode(401, new Response<string>
                        {
                            Success = false,
                            Errors = responseAuthentication.Errors
                        });
                    }
                    else
                    {
                        // Get the claims 
                        List<Claim> claims = _authenticationService.GetClaimsAdmin(responseAuthentication.Data);

                        // Generate the JWT
                        string token = _authenticationService.GenerateJwtToken(claims);

                        return StatusCode(200, new Response<string>
                        {
                            Success = true,
                            Data = token
                        });
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<string>
                    {
                        Success = false,
                        Errors = errors
                    });
                }
            }
            else
            {
                errors.Add("MAIL or PASSWORD incorrect");

                return StatusCode(400, new Response<string>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }

        /// <summary>
        /// Endpoint used to check the credentials of a driver and get a JWT back
        /// </summary>
        [AllowAnonymous]
        [Route("api/authentication/login/drivers/")]
        [HttpPost]
        public async Task<IActionResult> LoginDriverAsync([FromBody] LoginRequest loginRequest)
        {
            List<string> errors = new();

            // Check if the given model is valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Authenticate the user
                    Response<Driver> responseAuthentication = await _databaseService.Drivers.AuthenticateAsync(loginRequest);

                    // Check if the credentials are valid
                    if (!responseAuthentication.Success)
                    {
                        return StatusCode(401, new Response<string>
                        {
                            Success = false,
                            Errors = responseAuthentication.Errors
                        });
                    }
                    else
                    {
                        // Get the claims
                        List<Claim> claims = _authenticationService.GetClaimsDriver(responseAuthentication.Data);

                        // Generate the JWT
                        string token = _authenticationService.GenerateJwtToken(claims);

                        return StatusCode(200, new Response<string>
                        {
                            Success = true,
                            Data = token
                        });
                    }
                }
                catch (Exception e)
                {
                    errors.Add(e.Message);

                    return StatusCode(500, new Response<string>
                    {
                        Success = false,
                        Errors = errors
                    });
                }
            }
            else
            {
                errors.Add("Mail Or Password Incorrect");

                return StatusCode(400, new Response<string>
                {
                    Success = false,
                    Errors = errors
                });
            }
        }
    }
}
