namespace CarDealer.Web.Infrastructure.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using Services.Interfaces;

    public class Cache : ICache
    {
        private const string ManufacturersCacheKey = "_ManufacturersStoredInCache";
        private const string TransmissionTypesCacheKey = "_TransmissionTypesStoredInCache";
        private const string FuelTypesCacheKey = "_FuelTypesStoredInCache";
        
        private readonly IManufacturerService manufacturers;
        private readonly IVehicleElementService vehicleElements;
        private readonly IDistributedCache cache;

        public Cache(
            IManufacturerService manufacturers,
            IVehicleElementService vehicleElements,
            IDistributedCache cache)
        {
            this.manufacturers = manufacturers;
            this.vehicleElements = vehicleElements;
            this.cache = cache;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllManufacturersAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(ManufacturersCacheKey);
            if (listFromCache == null)
            {
                var allManufacturers = await this.manufacturers.AllAsync();
                list = allManufacturers.Select(m => new SelectListItem(m.Name.ToString(), m.Id.ToString()));
                var expiration = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays);

                await this.cache.SetSerializableObject(ManufacturersCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllTransmissionTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(TransmissionTypesCacheKey);
            if (listFromCache == null)
            {
                var transmissionTypes = await this.vehicleElements.GetTransmissionTypesAsync();
                list = transmissionTypes.Select(x => new SelectListItem(x.Name.ToString(), x.Id.ToString()));
                var expiration = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays);

                await this.cache.SetSerializableObject(TransmissionTypesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllFuelTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(FuelTypesCacheKey);
            if (listFromCache == null)
            {
                var fuelTypes = await this.vehicleElements.GetFuelTypesAsync();
                list = fuelTypes.Select(f => new SelectListItem(f.Name.ToString(), f.Id.ToString()));
                var expiration = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays);

                await this.cache.SetSerializableObject(FuelTypesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }
    }
}
