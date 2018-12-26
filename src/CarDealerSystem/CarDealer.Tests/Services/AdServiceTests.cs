namespace CarDealer.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using CarDealer.Services.Implementations.Ad;
    using CarDealer.Services.Models;
    using CarDealer.Services.Models.Ad;
    using CarDealer.Services.Models.Report;
    using CarDealer.Services.Models.User;
    using CarDealer.Services.Models.Vehicle;
    using Data;
    using FluentAssertions;
    using Models;
    using Xunit;

    public class AdServiceTests : BaseTest
    {
        private const string SampleManufacturerName = "BMW";
        private const string SampleModelName = "760";
        private const string SampleDescription = "SampleDescription";
        private const int SampleEngineHorsePower = 100;
        private const int SampleFuelConsumption = 10;
        private const int SampleYearOfManufacture = 2010;
        private const int SampleFuelTypeId = 1;
        private const int SampleTransmissionTypeId = 1;
        private const decimal SamplePrice = 10;
        private const string SampleUserId = "SampleUserId";
        private const string SamplePhoneNumber = "095235293583294234";

        private readonly CarDealerDbContext dbContext;
        private readonly AdService adService;

        public AdServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.adService = new AdService(this.dbContext, Mapper.Instance);
        }

        [Fact]
        public async Task CreateAsync_WithoutModel_ShouldReturnEmptyModelAndNotInsertAdInDatabase()
        {
            // Act
            var result = await this.adService.CreateAsync(null);
 
            // Assert
            result
                .Should()
                .BeAssignableTo<AdAndVehicleIds>()
                .And
                .Match(x => x.As<AdAndVehicleIds>().AdId == 0)
                .And
                .Match(x => x.As<AdAndVehicleIds>().VehicleId == 0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidManufacturerId_ShouldReturnEmptyModelAndNotInsertAdInDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var model = new VehicleCreateServiceModel
            {
                ModelName = SampleModelName,
            };

            // Act
            var result = await this.adService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeAssignableTo<AdAndVehicleIds>()
                .And
                .Match(x => x.As<AdAndVehicleIds>().AdId == 0)
                .And
                .Match(x => x.As<AdAndVehicleIds>().VehicleId == 0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModelName_ShouldReturnEmptyModelAndNotInsertAdInDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);

            // Act
            var model = new VehicleCreateServiceModel
            {
                ManufacturerId = 1,
            };
            var result = await this.adService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeAssignableTo<AdAndVehicleIds>()
                .And
                .Match(x => x.As<AdAndVehicleIds>().AdId == 0)
                .And
                .Match(x => x.As<AdAndVehicleIds>().VehicleId == 0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidEngineHorsePower_ShouldReturnEmptyModelAndNotInsertAdInDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var vehicleCreateServiceModel = this.InitializeValidVehicleCreateServiceModel();
            vehicleCreateServiceModel.EngineHorsePower = -1;
            // Act
            var result = await this.adService.CreateAsync(vehicleCreateServiceModel);
            // Assert
            result
                .Should()
                .BeAssignableTo<AdAndVehicleIds>()
                .And
                .Match(x => x.As<AdAndVehicleIds>().AdId == 0)
                .And
                .Match(x => x.As<AdAndVehicleIds>().VehicleId == 0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidYearOfManufacture_ShouldEmptyModelAndNotInsertAdInDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var vehicleCreateServiceModel = InitializeValidVehicleCreateServiceModel();
            vehicleCreateServiceModel.YearOfProduction = 1000;
            // Act
            var result = await this.adService.CreateAsync(vehicleCreateServiceModel);

            // Assert
            result
                .Should()
                .BeAssignableTo<AdAndVehicleIds>()
                .And
                .Match(x => x.As<AdAndVehicleIds>().AdId == 0)
                .And
                .Match(x => x.As<AdAndVehicleIds>().VehicleId == 0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidFuelConsumption_ShouldEmptyModelAndNotInsertAdInDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var vehicleCreateServiceModel = InitializeValidVehicleCreateServiceModel();
            vehicleCreateServiceModel.FuelConsumption = -1;
            // Act
            var result = await this.adService.CreateAsync(vehicleCreateServiceModel);

            // Assert
            result
                .Should()
                .BeAssignableTo<AdAndVehicleIds>()
                .And
                .Match(x => x.As<AdAndVehicleIds>().AdId == 0)
                .And
                .Match(x => x.As<AdAndVehicleIds>().VehicleId == 0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidTotalMileage_ShouldEmptyModelAndNotInsertAdInDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var vehicleCreateServiceModel = InitializeValidVehicleCreateServiceModel();
            vehicleCreateServiceModel.TotalMileage = -1;
            // Act
            var result = await this.adService.CreateAsync(vehicleCreateServiceModel);

            // Assert
            result
                .Should()
                .BeAssignableTo<AdAndVehicleIds>()
                .And
                .Match(x => x.As<AdAndVehicleIds>().AdId == 0)
                .And
                .Match(x => x.As<AdAndVehicleIds>().VehicleId == 0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidPrice_ShouldEmptyModelAndNotInsertAdInDatabase()
        {
            // Arrange
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName);
            var vehicleCreateServiceModel = InitializeValidVehicleCreateServiceModel();
            vehicleCreateServiceModel.Price = -10;
            // Act
            var result = await this.adService.CreateAsync(vehicleCreateServiceModel);

            // Assert
            result
                .Should()
                .BeAssignableTo<AdAndVehicleIds>()
                .And
                .Match(x => x.As<AdAndVehicleIds>().AdId == 0)
                .And
                .Match(x => x.As<AdAndVehicleIds>().VehicleId == 0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1000)]
        public async Task GetAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            SeedAds();

            // Act
            var result = await this.adService.GetAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        // Does not work properly with AutoMaper.QueryableExtensions
        //[Fact]
        //public async Task GetAsync_WithValidId_ShouldReturnCorrectEntity()
        //{
        //    // Arrange
        //    var ad = new Ad { Id = 1, };
        //    this.dbContext.Ads.Add(ad);
        //    this.dbContext.SaveChanges();

        //    // Act
        //    var result = await this.adService.GetAsync(1);

        //    // Assert
        //    result
        //        .Should()
        //        .Match<AdDetailsServiceModel>(a=> a.Id == 1);
        //}

        [Fact]
        public async Task GetAdOwnerEmail_WithValidId_ShouldReturnCorrectUser()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Ads.Add(new Ad { Id = id, IsDeleted = false, User = new User { Id = SampleUserId, Email = "SampleEmail", } });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.adService.GetAdOwnerEmail(id);

            // Assert
            result
                .Should()
                .BeAssignableTo<string>();
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task GetAdOwnerEmail_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            this.dbContext.Ads.Add(new Ad { Id = 1, IsDeleted = false, User = new User { Id = SampleUserId, Email = "SampleEmail", } });
            this.dbContext.Ads.Add(new Ad { Id = 2, IsDeleted = false, User = new User { Id = SampleUserId + 2, Email = "SampleEmail2", } });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.adService.GetAdOwnerEmail(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetAllAdsByOwnerId_ShouldReturnQueryWithValidModel()
        {
            // Arrange
            const string sampleUserEmail = "SampleUserEmail";
            SeedManufacturer(1, SampleManufacturerName);
            SeedModel(1, SampleModelName);
            this.dbContext.Ads.Add(new Ad
            {
                Id = 1,
                IsDeleted = false,
                IsReported = true,
                UserId = SampleUserId,
                CreationDate = DateTime.UtcNow,
                User = new User { Email = sampleUserEmail, Id = SampleUserId },
                Vehicle = new Vehicle
                {
                    Id = 1,
                    YearOfProduction = SampleYearOfManufacture,
                    Manufacturer = this.dbContext.Manufacturers.FirstOrDefault(),
                    Model = this.dbContext.Models.FirstOrDefault()
                }
            });
            this.dbContext.Ads.Add(new Ad
            {
                Id = 2,
                IsDeleted = false,
                IsReported = true,
                UserId = SampleUserId,
                CreationDate = DateTime.UtcNow,
                User = new User { Email = sampleUserEmail, Id = SampleUserId },
                Vehicle = new Vehicle
                {
                    Id = 2,
                    YearOfProduction = SampleYearOfManufacture,
                    Manufacturer = this.dbContext.Manufacturers.FirstOrDefault(),
                    Model = this.dbContext.Models.FirstOrDefault()
                }
            });

            this.dbContext.SaveChanges();

            // Act
            var result = await this.adService.GetAllAdsByOwnerId(SampleUserId);

            // Assert
            result
                .Should()
                .BeAssignableTo<IEnumerable<UserAdsListingServiceModel>>();
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task GetForUpdateAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            SeedAds();

            // Act
            var result = await this.adService.GetForUpdateAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Ads.Add(new Ad { Id = id, IsDeleted = false, Vehicle = new Vehicle { Id = id, } });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.adService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse(int id)
        {
            // Arrange
            SeedAds();
            // Act
            var result = await this.adService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnFalse(int id)
        {
            // Arrange
            SeedAds();
            SeedModel(1, SampleModelName);

            var model = InitializeValidAdEditServiceModel(id);
            // Act
            var result = await this.adService.UpdateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();

        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            const int id = 1;
            SeedAds();
            SeedModel(id, SampleModelName);
            var model = InitializeValidAdEditServiceModel(id);

            // Act
            var result = await this.adService.UpdateAsync(model);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidModel_ShouldReturnFalse()
        {
            // Arrange
            const int id = 1;
            SeedAds();
            var model = InitializeValidAdEditServiceModel(id);

            // Act
            var result = await this.adService.UpdateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task UpdateReportedAdAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Ads.Add(new Ad { Id = id, IsDeleted = true });
            this.dbContext.SaveChanges();
            // Act
            var result = await this.adService.UpdateReportedAdAsync(id);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task UpdateReportedAdAsync_WithInvalidId_ShouldReturnFalse(int id)
        {
            // Arrange
            SeedAds();

            // Act
            var result = await this.adService.UpdateReportedAdAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public void GetAllReportedAds_ShouldReturnQueryWithValidModel()
        {
            // Arrange
            const string sampleUserEmail = "SampleUserEmail";
            SeedManufacturer(1, SampleManufacturerName);
            SeedModel(1, SampleModelName);
            this.dbContext.Ads.Add(new Ad
            {
                Id = 1,
                IsDeleted = false,
                IsReported = true,
                UserId = SampleUserId,
                CreationDate = DateTime.UtcNow,
                User = new User { Email = sampleUserEmail, Id = SampleUserId },
                Vehicle = new Vehicle
                {
                    Id = 1,
                    YearOfProduction = SampleYearOfManufacture,
                    Manufacturer = this.dbContext.Manufacturers.FirstOrDefault(),
                    Model = this.dbContext.Models.FirstOrDefault()
                }
            });
            this.dbContext.Ads.Add(new Ad
            {
                Id = 2,
                IsDeleted = false,
                IsReported = true,
                UserId = SampleUserId,
                CreationDate = DateTime.UtcNow,
                User = new User { Email = sampleUserEmail, Id = SampleUserId },
                Vehicle = new Vehicle
                {
                    Id = 2,
                    YearOfProduction = SampleYearOfManufacture,
                    Manufacturer = this.dbContext.Manufacturers.FirstOrDefault(),
                    Model = this.dbContext.Models.FirstOrDefault()
                }
            });
            this.dbContext.Reports.Add(new Report { Id = 1, Description = SampleDescription, AdId = 1, });
            this.dbContext.Reports.Add(new Report { Id = 2, Description = SampleDescription, AdId = 2, });

            this.dbContext.SaveChanges();

            // Act
            var result = this.adService.GetAllReportedAds();

            // Assert
            result
                .Should()
                .BeAssignableTo<IQueryable<ReportListingServiceModel>>();
        }

        #region private methods

        private void SeedAds()
        {
            var ads = new List<Ad>();
            for (int i = 1; i < 10; i++)
            {
                ads.Add(new Ad { Id = i, IsDeleted = false, Vehicle = new Vehicle { Id = i, IsDeleted = false } });
            }

            this.dbContext.Ads.AddRange(ads);
            this.dbContext.SaveChanges();
        }

        private AdEditServiceModel InitializeValidAdEditServiceModel(int id)
        {
            var model = new AdEditServiceModel
            {
                Id = id,
                PhoneNumber = SamplePhoneNumber,
                UserId = SampleUserId,
                Vehicle = new VehicleEditServiceModel
                {
                    ManufacturerId = 1,
                    ModelName = SampleModelName,
                    Description = SampleDescription,
                    EngineHorsePower = SampleEngineHorsePower,
                    FuelConsumption = SampleFuelConsumption,
                    FuelTypeId = SampleFuelTypeId,
                    TransmissionTypeId = SampleTransmissionTypeId,
                    Price = SamplePrice,
                    FeatureIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                    YearOfProduction = SampleYearOfManufacture,
                }
            };

            return model;
        }

        private VehicleCreateServiceModel InitializeValidVehicleCreateServiceModel()
        {
            var model = new VehicleCreateServiceModel
            {
                ManufacturerId = 1,
                ModelName = SampleModelName,
                EngineHorsePower = SampleEngineHorsePower,
                FuelConsumption = SampleFuelConsumption,
                FuelTypeId = SampleFuelTypeId,
                TransmissionTypeId = SampleTransmissionTypeId,
                PhoneNumber = SamplePhoneNumber,
                UserId = SampleUserId,
                Price = SamplePrice,
                FeatureIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                YearOfProduction = SampleYearOfManufacture,
                Description = SampleDescription,
            };

            return model;
        }

        private void SeedModel(int id, string name)
        {
            var model = new Model
            {
                Id = id,
                Name = name,
                ManufacturerId = 1,
            };
            this.dbContext
                .Models
                .Add(model);
            this.dbContext.SaveChanges();
        }

        private void SeedManufacturer(int id, string name)
        {
            var manufacturer = new Manufacturer
            {
                Id = id,
                Name = name,
            };
            this.dbContext.Manufacturers.Add(manufacturer);
            this.dbContext.SaveChanges();
        }

        #endregion
    }
}
