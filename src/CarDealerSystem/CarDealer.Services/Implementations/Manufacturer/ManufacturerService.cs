namespace CarDealer.Services.Implementations.Manufacturer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using CarDealer.Models;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Manufacturer;

    public class ManufacturerService : BaseService, IManufacturerService
    {
        private readonly IMapper mapper;

        public ManufacturerService(CarDealerDbContext db, IMapper mapper)
            : base(db)
        {
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ManufacturerConciseListModel>> AllAsync()
            => await this.db
                .Manufacturers
                .OrderBy(m => m.Name)
                .ProjectTo<ManufacturerConciseListModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();

        public async Task<int> CreateAsync(string name)
        {
            var manufacturer = new Manufacturer { Name = name };
            
            try
            {
                this.ValidateEntityState(manufacturer);

                await this.db.AddAsync(manufacturer);
                await this.db.SaveChangesAsync();

                return manufacturer.Id;
            }
            catch
            {
                return default(int);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var manufacturer = await this.db.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return false;
            }

            this.db.Manufacturers.Remove(manufacturer);
            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<ManufacturerUpdateServiceModel> GetForUpdateAsync(int id)
        {
            var manufacturer = await this.db.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return null;
            }

            return this.mapper.Map<ManufacturerUpdateServiceModel>(manufacturer);
        }

        public async Task<ManufacturerDetailsServiceModel> GetDetailedAsync(int id)
            => await this.db
                .Manufacturers
                .ProjectTo<ManufacturerDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(m => m.Id == id);

        public async Task<bool> UpdateAsync(int id, string name)
        {
            var manufacturer = await this.db.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return false;
            }

            manufacturer.Name = name;

            try
            {
                this.ValidateEntityState(manufacturer);
                this.db.Manufacturers.Update(manufacturer);
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
