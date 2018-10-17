namespace CarDealer.Data
{
    using Configurations;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.BasicTypes;

    public class CarDealerDbContext : IdentityDbContext
    {
        public CarDealerDbContext()
        {
        }

        public CarDealerDbContext(DbContextOptions<CarDealerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Ad> Ads { get; set; }

        public DbSet<Feature> Features { get; set; }
        public DbSet<VehicleFeature> VehicleFeatures { get; set; }

        public DbSet<Report> Reports { get; set; }
        public DbSet<UserActivityLog> Logs { get; set; }

        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<TransmissionType> TransmissionTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=.;Database=CarDealerSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ManufacturerConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new ModelConfiguration());
            builder.ApplyConfiguration(new VehicleConfiguration());
            builder.ApplyConfiguration(new VehicleFeatureConfiguration());
            builder.ApplyConfiguration(new AdConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
