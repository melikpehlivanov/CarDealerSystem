namespace CarDealer.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Models.Vehicle;

    public interface IVehicleService
    {
        IQueryable<VehicleSearchServiceModel> Get(
            int yearOfManufacture, 
            int manufacturerId, 
            string modelName,
            int fuelTypeId, 
            int transmissionTypeId, 
            int minEngineHorsePower, 
            int maximumKilometers,
            decimal minPrice,
            decimal maxPrice);

        Task<IEnumerable<string>> GetByManufacturerIdAsync(int manufacturerId);

        Task<bool> CreateAsync(string modelName, int manufacturerId);

        Task<bool> DeleteAsync(int id);

        Task<ModelConciseServiceModel> GetAsync(int id);

        Task<bool> UpdateAsync(int id, string name);
    }
}
