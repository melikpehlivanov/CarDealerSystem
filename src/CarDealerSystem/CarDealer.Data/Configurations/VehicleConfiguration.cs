namespace CarDealer.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder
                .HasOne(v => v.Manufacturer)
                .WithMany(m => m.Vehicles)
                .HasForeignKey(v => v.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(v => v.Model)
                .WithMany(m => m.Vehicles)
                .HasForeignKey(v => v.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(v => v.Pictures)
                .WithOne(p => p.Vehicle)
                .HasForeignKey(v => v.VehicleId);
        }
    }
}
