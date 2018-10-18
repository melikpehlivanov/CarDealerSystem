namespace CarDealer.Services.Implementations.Vehicle
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using CarDealer.Models;
    using Common;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.Vehicle;

    public class VehicleService : BaseService, IVehicleService
    {
        private readonly IConfigurationProvider configuration;

        public VehicleService(CarDealerDbContext db, IMapper mapper)
            : base(db)
        {
            this.configuration = mapper.ConfigurationProvider;
        }

        public IQueryable<VehicleSearchServiceModel> Get(
            int yearOfManufacture,
            int manufacturerId,
            string modelName,
            int fuelTypeId,
            int transmissionTypeId,
            int minEngineHorsePower,
            int maximumKilometers,
            decimal minPrice,
            decimal maxPrice)
        {
            var modelNameSearchSubstring = modelName != null
                ? modelName != GlobalConstants.SearchTermForAllModels
                    ? modelName
                    : ""
                    : "";
            
            yearOfManufacture = yearOfManufacture != default(int) ? yearOfManufacture : 1990;
            maxPrice = maxPrice != default(int) ? maxPrice : int.MaxValue;

            if (manufacturerId == 1)
            {
                var series = modelNameSearchSubstring.Split(" ").First();

                var query = this.db
                    .Vehicles
                    .Where(v =>
                        !v.IsDeleted &&
                        v.ManufacturerId == manufacturerId &&
                        v.Model.Name.ToLower().StartsWith(series.ToLower()) &&
                        v.EngineHorsePower >= minEngineHorsePower &&
                        v.YearOfProduction >= yearOfManufacture &&
                        v.TotalMileage >= maximumKilometers &&
                        v.Price >= minPrice &&
                        v.Price <= maxPrice);

                if (fuelTypeId != default(int))
                {
                    query = query.Where(v => v.FuelTypeId == fuelTypeId);
                }
                if (transmissionTypeId != default(int))
                {
                    query = query.Where(v => v.TransmissionTypeId == transmissionTypeId);
                }

                return query
                    .ProjectTo<VehicleSearchServiceModel>(this.configuration)
                    .OrderBy(v => v.Id);
            }

            var vehicles = this.db
                .Vehicles
                .Include(v => v.Ads)
                .Where(v =>
                    !v.IsDeleted &&
                    v.ManufacturerId == manufacturerId &&
                    v.Model.Name.ToLower().Contains(modelNameSearchSubstring.ToLower()) &&
                    v.EngineHorsePower >= minEngineHorsePower &&
                    v.YearOfProduction >= yearOfManufacture &&
                    v.TotalMileage >= maximumKilometers &&
                    v.Price >= minPrice &&
                    v.Price <= maxPrice);

            if (fuelTypeId != default(int))
            {
                vehicles = vehicles.Where(v => v.FuelTypeId == fuelTypeId);
            }
            if (transmissionTypeId != default(int))
            {
                vehicles = vehicles.Where(v => v.TransmissionTypeId == transmissionTypeId);
            }

            return vehicles
                .ProjectTo<VehicleSearchServiceModel>(this.configuration)
                .OrderBy(v => v.Id);
        }

        public async Task<IEnumerable<string>> GetByManufacturerIdAsync(int manufacturerId)
            => await this.db
                .Models
                .Where(mod => mod.ManufacturerId == manufacturerId)
                .OrderBy(m => m.Name)
                .Select(mod => mod.Name)
                .ToListAsync();

        public async Task<bool> CreateAsync(string modelName, int manufacturerId)
        {
            var newModel = new Model
            {
                Name = modelName,
                ManufacturerId = manufacturerId
            };

            var modelExist = await this.db.Models.AnyAsync(m => m.ManufacturerId == manufacturerId && m.Name == modelName);
            if (modelExist)
            {
                return false;
            }

            try
            {
                this.ValidateEntityState(newModel);
                await this.db.Models.AddAsync(newModel);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var model = await this.db.Models.FindAsync(id);
            if (model == null)
            {
                return false;
            }

            this.db.Remove(model);
            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<ModelConciseServiceModel> GetAsync(int id)
        => await this.db
                .Models
                .ProjectTo<ModelConciseServiceModel>(this.configuration)
                .SingleOrDefaultAsync(m => m.Id == id);

        public async Task<bool> UpdateAsync(int id, string name)
        {
            var model = await this.db.Models.FirstOrDefaultAsync(m => m.Id == id);
            if (model == null)
            {
                return false;
            }

            try
            {
                model.Name = name;
                this.ValidateEntityState(model);
                this.db.Update(model);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
