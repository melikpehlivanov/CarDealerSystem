namespace CarDealer.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CarDealer.Models.BasicTypes;

    public interface IVehicleElementService
    {
        Task<IEnumerable<TransmissionType>> GetTransmissionTypes();
        Task<IEnumerable<FuelType>> GetFuelTypes();
        Task<List<Feature>> GetFeaturesByIdAsync(int id);
        Task<List<Feature>> GetFeaturesAsync();
    }
}
