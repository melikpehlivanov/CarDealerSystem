namespace CarDealer.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class ModelConfiguration : IEntityTypeConfiguration<Model>
    {
        public void Configure(EntityTypeBuilder<Model> builder)
        {
            builder
                .HasOne(m => m.Manufacturer)
                .WithMany(mf => mf.Models)
                .HasForeignKey(m => m.ManufacturerId);
        }
    }
}
