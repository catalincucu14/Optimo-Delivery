using Api.Database;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Authentication;
using Models.Resources;

#nullable disable

namespace Api.Services.Database.Drivers
{
    public class DriverService : IDriverService
    {
        private readonly DatabaseContext _databaseContext;

        public DriverService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<bool> ExistsAsync(int driverId, int adminId) => await _databaseContext.Drivers
                    .AnyAsync(driverTemp => driverTemp.DriverId == driverId && driverTemp.AdminId == adminId);

        /// <inheritdoc />
        public async Task<Response<Driver>> AuthenticateAsync(LoginRequest loginRequest)
        {
            List<string> errors = new();

            // Read the driver
            Driver driver = await _databaseContext.Drivers
                .Where(driverTemp => driverTemp.Mail == loginRequest.Mail)
                .FirstOrDefaultAsync();

            // Check an driver exists with that mail address
            if (driver is null)
            {
                errors.Add("MAIL or PASSWORD incorrect");

                return new Response<Driver>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Check if the password is correct
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, driver.Password))
                {
                    errors.Add("MAIL or PASSWORD incorrect");

                    return new Response<Driver>
                    {
                        Success = false,
                        Errors = errors
                    };
                }
                else
                {
                    return new Response<Driver>
                    {
                        Success = true,
                        Data = driver
                    };
                }
            }
        }

        /// <inheritdoc />
        public async Task<Response<List<Driver>>> ReadAllAsync(int adminId)
        {
            // Read the drivers
            List<Driver> drivers = await _databaseContext.Drivers
                .Where(driverTemp => driverTemp.AdminId == adminId)
                .Select(driverTemp => new Driver
                {
                    DriverId = driverTemp.DriverId,
                    AdminId = driverTemp.DriverId,
                    Mail = driverTemp.Mail,
                    Name = driverTemp.Name
                })
                .ToListAsync();

            return new Response<List<Driver>>
            {
                Success = true,
                Data = drivers
            };
        }

        /// <inheritdoc />
        public async Task<Response<Driver>> CreateAsync(int adminId, RegisterDriverRequest registerRequest)
        {
            List<string> errors = new();

            // Create driver from the register request
            Driver driver = new Driver
            {
                AdminId = adminId,
                Mail = registerRequest.Mail,
                Name = registerRequest.Name,
                Password = registerRequest.Password
            };

            // Check if the mail address is not already used by another driver
            if (await _databaseContext.Drivers.AnyAsync(driverTemp => driverTemp.Mail == driver.Mail))
            {
                errors.Add("MAIL already used");
            }

            // Check if the name is not already used by another driver from the same admin
            if (await _databaseContext.Drivers.AnyAsync(driverTemp => driverTemp.AdminId == adminId && driverTemp.Name == driver.Name))
            {
                errors.Add("NAME already used");
            }

            // Check if the password is empty, it should not be
            if (string.IsNullOrEmpty(driver.Password))
            {
                errors.Add("Password Required");
            }

            // Check if any of the above checks have triggered
            if (errors.Count > 0)
            {
                return new Response<Driver>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Hash the password
                driver.Password = BCrypt.Net.BCrypt.HashPassword(driver.Password, 12);

                // Create the driver
                await _databaseContext.Drivers.AddAsync(driver);

                // Save changes
                await _databaseContext.SaveChangesAsync();

                // Remove the password
                driver.Password = null;

                return new Response<Driver>
                {
                    Success = true,
                    Data = driver
                };
            }
        }

        /// <inheritdoc />
        public async Task<Response<Driver>> UpdateAsync(int driverId, int adminId, RegisterDriverRequest registerRequest)
        {
            List<string> errors = new();

            // Create driver from the register request
            Driver driver = new Driver
            {
                AdminId = adminId,
                Mail = registerRequest.Mail,
                Name = registerRequest.Name,
                Password = registerRequest.Password
            };

            // Read the driver
            Driver driverVar = await _databaseContext.Drivers
                .Where(driverTemp => driverTemp.AdminId == adminId && driverTemp.DriverId == driverId)
                .FirstAsync(); ;

            // Check if the mail address is not already used by another driver
            if (_databaseContext.Drivers.Any(driverTemp => driverTemp.Mail == driver.Mail && driverTemp.DriverId != driverId))
            {
                errors.Add("MAIL already used");
            }

            // Check if the name is not already used by another driver from the same admin
            if (_databaseContext.Drivers.Any(driverTemp => driverTemp.Name == driver.Name && driverTemp.AdminId == adminId && driverTemp.DriverId != driverId))
            {
                errors.Add("NAME already used");
            }

            // Check if any of the above checks have triggered
            if (errors.Count > 0)
            {
                return new Response<Driver>
                {
                    Success = false,
                    Errors = errors
                };
            }
            else
            {
                // Update the driver
                driverVar.Mail = driver.Mail;
                driverVar.Name = driver.Name;

                // Check if the password is empty, if so don't update the password
                if (!string.IsNullOrEmpty(driver.Password))
                {
                    // Hash the password
                    driverVar.Password = BCrypt.Net.BCrypt.HashPassword(driver.Password, 12);
                }

                // Save changes
                await _databaseContext.SaveChangesAsync();

                // Remove the password
                driverVar.Password = null;

                return new Response<Driver>
                {
                    Success = true,
                    Data = driverVar
                };
            }
        }
    }
}
