namespace CarDealer.Services.Implementations.Ad
{
    using System;
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
    using Models.Ad;
    using Models.Report;
    using Models.User;
    using Models.Vehicle;

    public class AdService : BaseService, IAdService
    {
        private readonly IMapper mapper;

        public AdService(CarDealerDbContext db, IMapper mapper)
            : base(db)
        {
            this.mapper = mapper;
        }

        public async Task<int> CreateAsync(VehicleCreateServiceModel model)
        {
            if (model == null)
            {
                return default(int);
            }

            var manufacturerExist = await this.db.Manufacturers.AnyAsync(m => m.Id == model.ManufacturerId);
            if (!manufacturerExist)
            {
                return default(int);
            }

            var newVehicle = this.mapper.Map<Vehicle>(model);

            newVehicle.Model = await this.db.Models
                .FirstOrDefaultAsync(m => m.ManufacturerId == model.ManufacturerId && m.Name == model.ModelName);

            if (newVehicle.Model == null)
            {
                return default(int);
            }

            var vehicleFeatures = new List<VehicleFeature>();
            foreach (var modelFeatureId in model.FeatureIds)
            {
                var feature = new VehicleFeature
                {
                    FeatureId = modelFeatureId,
                    Vehicle = newVehicle
                };
                vehicleFeatures.Add(feature);
            }

            var newAd = new Ad
            {
                CreationDate = DateTime.UtcNow,
                Vehicle = newVehicle,
                UserId = model.UserId,
                PhoneNumber = model.PhoneNumber
            };

            try
            {
                this.ValidateEntityState(newVehicle);
                this.ValidateEntityState(newAd);

                await this.db.AddAsync(newAd);
                await this.db.AddRangeAsync(vehicleFeatures);
                await this.db.SaveChangesAsync();
            }
            catch
            {
                return default(int);
            }

            return newVehicle.Id;
        }

        public async Task<AdDetailsServiceModel> GetAsync(int id)
            => await this.db
                .Ads
                .Where(a => a.Id == id)
                .ProjectTo<AdDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(a=> a.Id == id);

        public string GetAdOwnerEmail(int id)
            => this.db
                .Ads
                .Where(a => a.Id == id)
                .Select(a => a.User.Email)
                .SingleOrDefault();

        public IQueryable<UserAdsListingServiceModel> GetAllAdsByOwnerId(string id)
            => this.db
                .Ads
                .Where(a => a.UserId == id && !a.IsDeleted)
                .ProjectTo<UserAdsListingServiceModel>(this.mapper.ConfigurationProvider);

        public async Task<AdEditServiceModel> GetForUpdateAsync(int id)
            => await this.db
                .Ads
                .Where(v => v.Id == id)
                .ProjectTo<AdEditServiceModel>(this.mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

        public async Task<bool> DeleteAsync(int id)
        {
            var ad = await this.db
                .Ads
                .Include(v => v.Vehicle)
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            if (ad == null)
            {
                return false;
            }

            try
            {
                ad.IsDeleted = true;
                ad.Vehicle.IsDeleted = true;

                this.db.Update(ad);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(AdEditServiceModel serviceModel)
        {
            if (serviceModel == null)
            {
                return false;
            }

            try
            {
                var ad = await this.db
                    .Ads
                    .Include(v => v.Vehicle)
                    .SingleOrDefaultAsync(a => a.Id == serviceModel.Id);
                var modelId = await this.db
                    .Models
                    .Where(m => m.Name == serviceModel.Vehicle.ModelName)
                    .Select(m => m.Id)
                    .SingleOrDefaultAsync();

                if (ad == null || modelId == default(int))
                {
                    return false;
                }

                var vehicle = ad.Vehicle;

                ad.PhoneNumber = serviceModel.PhoneNumber;

                vehicle.ModelId = modelId;
                vehicle.Description = serviceModel.Vehicle.Description;
                vehicle.ManufacturerId = serviceModel.Vehicle.ManufacturerId;
                vehicle.FuelTypeId = serviceModel.Vehicle.FuelTypeId;
                vehicle.TransmissionTypeId = serviceModel.Vehicle.TransmissionTypeId;
                vehicle.Engine = serviceModel.Vehicle.Engine;
                vehicle.EngineHorsePower = serviceModel.Vehicle.EngineHorsePower;
                vehicle.YearOfProduction = serviceModel.Vehicle.YearOfProduction;
                vehicle.TotalMileage = serviceModel.Vehicle.TotalMileage;
                vehicle.FuelConsumption = serviceModel.Vehicle.FuelConsumption;
                vehicle.Price = serviceModel.Vehicle.Price;
                vehicle.Pictures = serviceModel.Vehicle.Pictures;

                var vehicleFeatures = new List<VehicleFeature>();
                foreach (var featureId in serviceModel.Vehicle.FeatureIds)
                {
                    var vehicleFeature = new VehicleFeature
                    {
                        FeatureId = featureId,
                    };

                    vehicleFeatures.Add(vehicleFeature);
                }

                if (!vehicleFeatures.Any())
                {
                    return false;
                }

                vehicle.Features = vehicleFeatures;

                await DeleteFeaturesByVehicleIdAsync(serviceModel); //TODO implement logic to check if the Id exists and to skip instead of deleting it.

                this.db.Update(vehicle);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateReportedAdAsync(int id)
        {
            var ad = await this.db
                .Ads
                .FindAsync(id);

            if (ad == null)
            {
                return false;
            }
            
            try
            {
                ad.IsReported = false;

                this.db.Update(ad);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IQueryable<ReportListingServiceModel> GetAllReportedAds()
        {
            var reports = this.db
                .Reports
                .ToList();

            var ads = this.db
                .Ads
                .Where(a => a.IsReported && !a.IsDeleted)
                .Select(a => new ReportListingServiceModel
                {
                    CreationDate = a.CreationDate,
                    Description = reports.Where(r => r.AdId == a.Id).Select(r => r.Description).FirstOrDefault(),
                    UserEmail = a.User.Email,
                    Id = a.Id,
                    VehicleId = a.Vehicle.Id,
                    FullModelName = $"{a.Vehicle.YearOfProduction} {a.Vehicle.Manufacturer.Name} {a.Vehicle.Model.Name}",
                    PicturePath = a.Vehicle.Pictures.Any() ? a.Vehicle.Pictures.First().Path : GlobalConstants.DefaultPicturePath
                });

            return ads;

        }

        private async Task DeleteFeaturesByVehicleIdAsync(AdEditServiceModel serviceModel)
        {
            var features = await this.db
                                .VehicleFeatures
                                .Where(v => v.VehicleId == serviceModel.Id)
                                .ToListAsync();

            this.db.VehicleFeatures.RemoveRange(features);

            await this.db.SaveChangesAsync();
        }
    }
}
