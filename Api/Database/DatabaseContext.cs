using Microsoft.EntityFrameworkCore;
using Models.Resources;
using Route = Models.Resources.Route;

#nullable disable

namespace Api.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Route> Routes { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Admin>()
                .HasIndex(adminTemp => adminTemp.Mail)
                .IsUnique();

            builder.Entity<Admin>()
                .HasIndex(adminTemp => adminTemp.Phone)
                .IsUnique();

            builder.Entity<Driver>()
                .HasIndex(driverTemp => driverTemp.Mail)
                .IsUnique();

            builder.Entity<Driver>()
               .HasIndex(driverTemp => new { driverTemp.AdminId, driverTemp.Name })
               .IsUnique();

            builder.Entity<Order>()
                .HasIndex(orderTemp => new { orderTemp.AdminId, orderTemp.CustomId })
                .IsUnique();
        }
    }
}
