namespace CarDealer.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Models;

    public class AdConfiguration : IEntityTypeConfiguration<Ad>
    {
        public void Configure(EntityTypeBuilder<Ad> builder)
        {
            builder
                .HasMany(r => r.Reports)
                .WithOne(a => a.Ad)
                .HasForeignKey(a => a.AdId);
        }
    }
}
