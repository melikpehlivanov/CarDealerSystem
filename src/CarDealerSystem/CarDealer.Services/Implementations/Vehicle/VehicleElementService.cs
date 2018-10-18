namespace CarDealer.Services.Implementations.Vehicle
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Models.BasicTypes;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class VehicleElementService : BaseService, IVehicleElementService
    {
        public VehicleElementService(CarDealerDbContext db) :
            base(db)
        {
        }

        public async Task<IEnumerable<FuelType>> GetFuelTypesAsync()
            => await this.db.FuelTypes.ToListAsync();

        public async Task<List<Feature>> GetFeaturesByIdAsync(int id)
            => await this.db
                .VehicleFeatures
                .Where(v => v.VehicleId == id)
                .Select(v => new Feature
                {
                    Id = v.Feature.Id,
                    Name = v.Feature.Name,
                    IsChecked = true,
                })
                .ToListAsync();

        public async Task<List<Feature>> GetFeaturesAsync()
            => await this.db.Features.ToListAsync();

        public async Task<IEnumerable<TransmissionType>> GetTransmissionTypesAsync()
            => await this.db.TransmissionTypes.ToListAsync();
    }
}
