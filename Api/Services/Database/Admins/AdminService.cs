using Api.Database;
using ISO3166;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Authentication;
using Models.Resources;

#nullable disable

namespace Api.Services.Database.Admins
{
    public class AdminService : IAdminService
    {
        private readonly DatabaseContext _databaseContext;

        public AdminService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int adminId) => await _databaseContext.Admins
            .AnyAsync(adminTemp => adminTemp.AdminId == adminId);

        /// <inheritdoc />
        public async Task<Response<Admin>> AuthenticateAsync(LoginRequest loginRequest)
        {
            List<string> errors = new();

            // Read the admin
            Admin admin = await _databaseContext.Admins
                .Where(adminTemp => adminTemp.Mail == loginRequest.Mail)
                .FirstOrDefaultAsync();

            // Check an admin exists with that mail address
            if (admin is null)
            {
                errors.Add("MAIL or PASSWORD incorrect");

                return new Response<Admin>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Check if the password is correct
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, admin.Password))
                {
                    errors.Add("MAIL or PASSWORD incorrect");

                    return new Response<Admin>
                    {
                        Success = false,
                        Errors = errors
                    };
                }
                else
                {
                    return new Response<Admin>
                    {
                        Success = true,
                        Data = admin
                    };
                }
            }
        }

        /// <inheritdoc />
        public async Task<Response<Admin>> ReadOneAsync(int adminId)
        {
            List<string> errors = new();

            // Read the admin 
            Admin admin = await _databaseContext.Admins
                .Where(adminTemp => adminTemp.AdminId == adminId)
                .Select(adminTemp => new Admin
                {
                    AdminId = adminTemp.AdminId,
                    Mail = adminTemp.Mail,
                    Phone = adminTemp.Phone,
                    Country = adminTemp.Country
                })
                .FirstAsync();

            return new Response<Admin>
            {
                Success = true,
                Data = admin
            };
        }

        /// <inheritdoc />
        public async Task<Response<Admin>> CreateAsync(RegisterAdminRequest registerRequest)
        {
            List<string> errors = new();

            // Create admin from the register request
            Admin admin = new Admin
            {
                Mail = registerRequest.Mail,
                Phone = registerRequest.Phone,
                Country = registerRequest.Country,
                Password = registerRequest.Password
            };

            // Check if the mail address is not already used by another admin
            if (await _databaseContext.Admins.AnyAsync(adminTemp => adminTemp.Mail == admin.Mail))
            {
                errors.Add("MAIL already used");
            }

            // Check if the phone number is not already used by another admin
            if (await _databaseContext.Admins.AnyAsync(adminTemp => adminTemp.Phone == admin.Phone))
            {
                errors.Add("PHONE already used");
            }

            // Check if the country name is valid in order to prevent silly input
            if (Country.List.Where(countryTemp => countryTemp.Name == admin.Country).ToList().Count == 0)
            {
                errors.Add("COUNTRY invalid");
            }

            // Check if any of the above checks have triggered
            if (errors.Count > 0)
            {
                return new Response<Admin>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Hash the password
                admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password, 12);

                // Create the admin
                await _databaseContext.Admins.AddAsync(admin);

                // Save changes
                await _databaseContext.SaveChangesAsync();

                // Remove the password
                admin.Password = null;

                return new Response<Admin>
                {
                    Success = true,
                    Data = admin
                };
            }
        }
    }
}
