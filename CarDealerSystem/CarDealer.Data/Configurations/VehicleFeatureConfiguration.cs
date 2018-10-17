namespace CarDealer.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class VehicleFeatureConfiguration : IEntityTypeConfiguration<VehicleFeature>
    {
        public void Configure(EntityTypeBuilder<VehicleFeature> builder)
        {
            builder
                .HasKey(vf => new {vf.VehicleId, vf.FeatureId});

            builder
                .HasOne(v => v.Vehicle)
                .WithMany(f => f.Features)
                .HasForeignKey(v => v.VehicleId);

            builder
                .HasOne(f => f.Feature)
                .WithMany(v => v.Vehicles)
                .HasForeignKey(f => f.FeatureId);
        }
    }
}
