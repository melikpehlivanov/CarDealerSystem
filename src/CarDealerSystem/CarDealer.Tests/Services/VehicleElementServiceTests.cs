namespace CarDealer.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CarDealer.Services.Implementations.Vehicle;
    using Data;
    using FluentAssertions;
    using Models;
    using Models.BasicTypes;
    using Xunit;

    public class VehicleElementServiceTests : BaseTest
    {
        private readonly CarDealerDbContext dbContext;
        private readonly VehicleElementService vehicleElementService;

        public VehicleElementServiceTests()
        {
            this.dbContext = this.DatabaseInstance;
            this.vehicleElementService = new VehicleElementService(this.dbContext);
        }

        [Fact]
        public async Task GetFeaturesByIdAsync_ShouldReturnCorrectModel()
        {
            // Arrange
            this.SeedTypes<Feature>(10);
            this.dbContext.Vehicles.Add(new Vehicle { Id = 1, IsDeleted = false, });
            this.dbContext.VehicleFeatures.Add(new VehicleFeature { VehicleId = 1, FeatureId = 1 });
            this.dbContext.SaveChanges();
            // Act
            var result = await this.vehicleElementService.GetFeaturesByIdAsync(1);

            // Assert
            result
                .Should()
                .AllBeAssignableTo<Feature>();
        }
        
        [Fact]
        public async Task GetFuelTypesAsync_ShouldReturnCorrectModel()
        {
            // Arrange
            this.dbContext.FuelTypes.Add(new FuelType());
            // Act
            var result = await this.vehicleElementService.GetFuelTypesAsync();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<FuelType>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task GetFuelTypesAsync_ShouldReturnCorrectCount(int count)
        {
            // Arrange
            this.SeedTypes<FuelType>(count);

            // Act
            var result = await this.vehicleElementService.GetFuelTypesAsync();

            // Assert
            result
                .Should()
                .HaveCount(count);
        }

        [Fact]
        public async Task GetTransmissionTypesAsync_ShouldReturnCorrectModel()
        {
            // Arrange
            this.dbContext.TransmissionTypes.Add(new TransmissionType());
            this.dbContext.SaveChanges();
            // Act
            var result = await this.vehicleElementService.GetTransmissionTypesAsync();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<TransmissionType>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task GetTransmissionTypesAsync_ShouldReturnCorrectCount(int count)
        {
            // Arrange
            this.SeedTypes<TransmissionType>(count);
            // Act
            var result = await this.vehicleElementService.GetTransmissionTypesAsync();

            // Assert
            result
                .Should()
                .HaveCount(count);
        }

        [Fact]
        public async Task GetFeaturesAsync_ShouldReturnCorrectModel()
        {
            // Arrange
            this.dbContext.Features.Add(new Feature());
            this.dbContext.SaveChanges();
            // Act
            var result = await this.vehicleElementService.GetFeaturesAsync();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<Feature>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task GetFeaturesAsync_ShouldReturnCorrectCount(int count)
        {
            // Arrange
            this.SeedTypes<Feature>(count);
            // Act
            var result = await this.vehicleElementService.GetFeaturesAsync();

            // Assert
            result
                .Should()
                .HaveCount(count);
        }

        private void SeedTypes<T>(int count)
            where T : class
        {
            var collection = new List<T>();
            for (int i = 1; i <= count; i++)
            {
                var instance = Activator.CreateInstance<T>();
                collection.Add(instance);
            }
            this.dbContext.Set<T>().AddRange(collection);
            this.dbContext.SaveChanges();
        }
    }
}
