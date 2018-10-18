namespace CarDealer.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CarDealer.Models.BasicTypes;

    public interface IVehicleElementService
    {
        Task<IEnumerable<TransmissionType>> GetTransmissionTypesAsync();
        Task<IEnumerable<FuelType>> GetFuelTypesAsync();
        Task<List<Feature>> GetFeaturesByIdAsync(int id);
        Task<List<Feature>> GetFeaturesAsync();
    }
}
