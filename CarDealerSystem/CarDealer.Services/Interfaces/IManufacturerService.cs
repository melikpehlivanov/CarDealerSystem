namespace CarDealer.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Models.Manufacturer;

    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerConciseListModel>> AllAsync();

        Task<ManufacturerUpdateServiceModel> GetForUpdateAsync(int id);

        Task<int> CreateAsync(string name);

        Task<bool> UpdateAsync(int id, string name);

        Task<bool> DeleteAsync(int id);

        Task<ManufacturerDetailsServiceModel> GetDetailedAsync(int id);
    }
}
