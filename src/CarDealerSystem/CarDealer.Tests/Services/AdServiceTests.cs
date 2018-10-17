namespace CarDealer.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using CarDealer.Services.Implementations.Ad;
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
        private const int SampleEngineHorsePower = 100;
        private const int SampleFuelConsumption = 10;
        private const int SampleYearOfManufacture = 2010;
        private const int SampleFuelTypeId = 1;
        private const int SampleTransmissionTypeId = 1;
        private const decimal SamplePrice = 10;
        private const string SampleUserId = "SampleUser";
        private const string SamplePhoneNumber = "095235293583294234";

        private readonly CarDealerDbContext dbContext;
        private readonly AdService adService;

        public AdServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.adService = new AdService(this.dbContext, Mapper.Instance);
        }

        [Fact]
        public async Task CreateAsync_WithoutModel_ShouldReturnZeroAndNotInsertAdInDatabase()
        {
            // Act
            var result = await this.adService.CreateAsync(null);

            // Assert
            result
                .Should()
                .Be(0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidManufacturerId_ShouldReturnZeroAndNotInsertAdInDatabase()
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
                .Be(0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModelName_ShouldReturnZeroAndNotInsertAdInDatabase()
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
                .Be(0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidEngineHorsePower_ShouldReturnZeroAndNotInsertAdInDatabase()
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
                .Be(0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidYearOfManufacture_ShouldReturnZeroAndNotInsertAdInDatabase()
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
                .Be(0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidFuelConsumption_ShouldReturnZeroAndNotInsertAdInDatabase()
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
                .Be(0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidTotalMileage_ShouldReturnZeroAndNotInsertAdInDatabase()
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
                .Be(0);

            this.dbContext
                .Ads
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidPrice_ShouldReturnZeroAndNotInsertAdInDatabase()
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
                .Be(0);

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
            var ad = new Ad { Id = 1, };
            this.dbContext.Ads.Add(ad);
            this.dbContext.SaveChanges();

            // Act
            var result = await this.adService.GetAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public void GetAllAdsByOwnerId_ShouldReturnQueryWithValidModel()
        {
            this.dbContext.Ads.Add(new Ad { Id = 1, IsDeleted = false, UserId = SampleUserId });
            this.dbContext.Ads.Add(new Ad { Id = 2, IsDeleted = false, UserId = SampleUserId });
            
            this.dbContext.SaveChanges();

            var result = this.adService.GetAllAdsByOwnerId(SampleUserId);

            result
                .Should()
                .BeAssignableTo<IQueryable<UserAdsListingServiceModel>>();

        }

        #region private methods

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
