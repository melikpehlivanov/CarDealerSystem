namespace CarDealer.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using CarDealer.Services.Implementations.Vehicle;
    using CarDealer.Services.Models;
    using CarDealer.Services.Models.Vehicle;
    using Data;
    using FluentAssertions;
    using Models;
    using Models.BasicTypes;
    using Xunit;

    public class VehicleServiceTests : BaseTest
    {
        private const string SampleManufacturerName = "BMW";
        private const string SampleModelName = "760Li";
        private const int SampleTotalMileage = 1000;
        private const string Diesel = "Diesel";
        private const int SampleEngineHorsePower = 610;
        private const int SampleTransmissionTypeId = 1;
        private const int SampleFuelTypeId = 1;
        private const int SampleYearOfManufacture = 2018;

        private readonly CarDealerDbContext dbContext;
        private readonly VehicleService vehicleService;

        public VehicleServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.vehicleService = new VehicleService(this.dbContext, Mapper.Instance);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidArguments_ShouldReturnFalse()
        {
            // Act
            var result = await this.vehicleService.CreateAsync(null, default(int));

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithValidArguments_ShouldReturnTrueAndAddModelInDatabase()
        {
            // Arrange 
            this.SeedManufacturer(1, SampleManufacturerName);

            // Act
            var result = await this.vehicleService.CreateAsync(SampleModelName, 1);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Models
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithAlreadyExistingModel_ShouldReturnFalse()
        {
            // Arrange
            const int id = 1;
            this.SeedManufacturer(id, SampleManufacturerName);
            this.SeedModel(id, SampleModelName, id);
            // Act
            var result = await this.vehicleService.CreateAsync(SampleModelName, id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Get_ShouldReturnCorrectModel(int id)
        {
            // Arrange
            this.SeedManufacturer(id, SampleManufacturerName);
            this.SeedModel(1, SampleModelName, id);
            this.SeedVehicles(10);

            // Act
            var result = this.vehicleService
                .Get(1, id, SampleModelName, SampleFuelTypeId, SampleTransmissionTypeId, 0, 0, decimal.MinValue, decimal.MaxValue)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNullOrEmpty()
                .And
                .AllBeAssignableTo<VehicleSearchServiceModel>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Get_ShouldReturnCorrectCountOfVehicles(int id)
        {
            // Arrange
            const int vehiclesCount = 10;
            this.SeedManufacturer(id, SampleManufacturerName);
            this.SeedModel(1, SampleModelName, id);
            this.SeedVehicles(vehiclesCount);

            // Act
            var result = this.vehicleService
                .Get(1, id, SampleModelName, SampleFuelTypeId, SampleTransmissionTypeId, 0, 0, decimal.MinValue, decimal.MaxValue)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(vehiclesCount);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Get_ShouldFilterManufacturerCorrectly(int id)
        {
            // Arrange
            const int notMatchedVehicleId = 3;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(id, SampleManufacturerName);
            this.SeedManufacturer(3, "another manufacturer");
            this.SeedModel(1, SampleModelName, id);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleId,
                    ManufacturerId = 3,
                    ModelId = 1,
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, id, SampleModelName, SampleFuelTypeId, SampleTransmissionTypeId, 0, 0, decimal.MinValue, decimal.MaxValue)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleId));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Get_ShouldFilterModelCorrectly(int id)
        {
            // Arrange
            const int notMatchedVehicleId = 3;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(id, SampleManufacturerName);
            this.SeedModel(1, SampleModelName, id);
            this.SeedModel(3, "Some another test model", 3);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleId,
                    ManufacturerId = 3,
                    ModelId = 2,
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, id, SampleModelName, SampleFuelTypeId, SampleTransmissionTypeId, 0, 0, decimal.MinValue, decimal.MaxValue)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleId));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Get_ShouldFilterEngineHorsePowerCorrectly(int id)
        {
            // Arrange
            const int notMatchedVehicleIdOne = 10;
            const int notMatchedVehicleIdTwo = 11;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(id, SampleManufacturerName);
            this.SeedModel(1, SampleModelName, id);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdOne,
                    ManufacturerId = id,
                    ModelId = 1,
                    EngineHorsePower = 610,
                });
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdTwo,
                    ManufacturerId = id,
                    ModelId = 1,
                    EngineHorsePower = 690,
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, id, SampleModelName, SampleFuelTypeId, SampleTransmissionTypeId, SampleEngineHorsePower, 0, decimal.MinValue, decimal.MaxValue)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleIdOne))
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleIdTwo));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Get_ShouldFilterYearOfManufactureCorrectly(int id)
        {
            // Arrange
            const int notMatchedVehicleIdOne = 3;
            const int notMatchedVehicleIdTwo = 4;
            const int expectedVehiclesCount = 2;
            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName, id);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Vehicles
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdOne,
                    ManufacturerId = 1,
                    ModelId = 1,
                    YearOfProduction = SampleYearOfManufacture - 1
                });
            this.dbContext
                .Vehicles
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdTwo,
                    ManufacturerId = 1,
                    ModelId = 1,
                    YearOfProduction = SampleYearOfManufacture + 1,
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(SampleYearOfManufacture, 1, SampleModelName, SampleFuelTypeId, SampleTransmissionTypeId, 10, 10, decimal.MinValue, decimal.MaxValue)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleIdOne))
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleIdTwo));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Get_ShouldFilterTotalMileageCorrectly(int id)
        {
            // Arrange
            const int notMatchedVehicleId = 3;
            const int notMatchedVehicleIdTwo = 4;
            const int expectedVehiclesCount = 2;

            this.SeedManufacturer(1, SampleManufacturerName);
            this.SeedModel(1, SampleModelName, id);
            this.SeedVehicles(expectedVehiclesCount);
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleId,
                    ManufacturerId = 1,
                    ModelId = 1,
                    TotalMileage = SampleTotalMileage - 10
                });
            this.dbContext
                .Add(new Vehicle
                {
                    Id = notMatchedVehicleIdTwo,
                    ManufacturerId = 1,
                    ModelId = 1,
                    TotalMileage = SampleTotalMileage - 1
                });
            this.dbContext.SaveChanges();

            // Act
            var result = this.vehicleService
                .Get(1, 1, SampleModelName, SampleFuelTypeId, SampleTransmissionTypeId, 10, SampleTotalMileage, decimal.MinValue, decimal.MaxValue)
                .ToList();

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .HaveCount(expectedVehiclesCount)
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.All(v => v.Id != notMatchedVehicleId))
                .And
                .Match<List<VehicleSearchServiceModel>>(l => l.Any(v => v.Id != notMatchedVehicleIdTwo));
        }

        [Fact]
        public async Task GetByManufacturerIdAsync_WithValidId_ShouldReturnCollectionOfStrings()
        {
            // Arrange
            const int id = 1;
            this.SeedModel(id, SampleModelName, 1);
            this.SeedManufacturer(id, SampleManufacturerName);
            // Act
            var result = await this.vehicleService.GetByManufacturerIdAsync(id);

            // Assert
            result
                .Should()
                .NotBeNullOrEmpty()
                .And
                .AllBeAssignableTo<string>();
        }

        [Fact]
        public async Task GetByManufacturerIdAsync_WithInvalidId_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await this.vehicleService.GetByManufacturerIdAsync(default(int));

            // Assert
            result
                .Should()
                .BeNullOrEmpty();
        }

        [Fact]
        public async Task GetByManufacturerIdAsync_ShouldReturnOrderedByNameCollection()
        {
            // Arrange
            for (int i = 1; i <= 10; i++)
            {
                this.SeedModel(i, $"Model_{i}", i);
                this.SeedManufacturer(i, $"Manufacturer_{i}");
            }

            // Act
            var result = await this.vehicleService.GetByManufacturerIdAsync(1);

            // Assert
            result
                .Should()
                .BeInAscendingOrder();
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            this.SeedModel(1, SampleModelName, 1);
            this.SeedManufacturer(1, SampleManufacturerName);
            // Act
            var result = await this.vehicleService.DeleteAsync(1);

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
            const int count = 4;
            for (int i = 1; i <= count; i++)
            {
                this.SeedModel(i, $"Model_{i}", i);
                this.SeedManufacturer(i, $"Manufacturer_{i}");
            }

            // Act
            var result = await this.vehicleService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task GetAsync_WithValidId_ShouldReturnCorrectEntity()
        {
            // Arrange
            this.SeedModel(1, SampleModelName, 1);
            this.SeedManufacturer(1, SampleManufacturerName);

            // Act
            var result = await this.vehicleService.GetAsync(1);

            // Assert
            result
                .Should()
                .BeAssignableTo<ModelConciseServiceModel>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task GetAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            this.SeedModel(1, SampleModelName, 1);
            this.SeedManufacturer(1, SampleManufacturerName);

            // Act
            var result = await this.vehicleService.GetAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task UpdateAsync_WithValidArguments_ShouldReturnTrue()
        {
            // Arrange
            const int id = 1;
            const string newName = "NewName";

            this.SeedModel(id, SampleModelName, 1);
            this.SeedManufacturer(id, SampleManufacturerName);

            // Act
            var result = await this.vehicleService.UpdateAsync(id, newName);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task UpdateAsync_WithInvalidArguments_ShouldReturnFalse(string name)
        {
            // Arrange
            const int id = 1;

            this.SeedModel(id, SampleModelName, 1);
            this.SeedManufacturer(id, SampleManufacturerName);

            // Act
            var result = await this.vehicleService.UpdateAsync(id, name);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        #region privateMethods

        private void SeedVehicles(int vehiclesCount)
        {
            var vehiclesToSeed = new List<Vehicle>();
            var ads = new List<Ad>();
            var fuelType = new FuelType{Id = 1, Name = Diesel};
            var manufacturer = this.dbContext.Manufacturers.FirstOrDefault();
            for (int i = 1; i <= vehiclesCount; i++)
            {
                ads.Add(new Ad { Id = i });
                vehiclesToSeed.Add(new Vehicle
                {
                    Id = i,
                    Ads = ads,
                    Manufacturer = manufacturer,
                    Model = this.dbContext.Models.FirstOrDefault(),
                    ManufacturerId = manufacturer.Id,
                    ModelId = 1,
                    EngineHorsePower = SampleEngineHorsePower,
                    YearOfProduction = SampleYearOfManufacture,
                    FuelTypeId = 1,
                    FuelType = fuelType,
                    TransmissionTypeId = 1,
                    Price = 10,
                    TotalMileage = SampleTotalMileage,
                });
            }

            this.dbContext.AddRange(vehiclesToSeed);
            this.dbContext.SaveChanges();
        }

        private void SeedModel(int id, string modelName, int manufacturerId)
        {
            this.dbContext
                .Add(new Model
                {
                    Id = id,
                    Name = modelName,
                    ManufacturerId = manufacturerId
                });

            this.dbContext.SaveChanges();
        }

        private void SeedManufacturer(int id, string manufacturerName)
        {
            this.dbContext
                .Add(new Manufacturer
                {
                    Id = id,
                    Name = manufacturerName
                });

            this.dbContext.SaveChanges();
        }

        #endregion
    }
}
