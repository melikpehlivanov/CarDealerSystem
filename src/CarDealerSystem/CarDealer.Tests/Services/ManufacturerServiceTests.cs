namespace CarDealer.Tests.Services
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using CarDealer.Services.Implementations.Manufacturer;
    using CarDealer.Services.Models.Manufacturer;
    using Data;
    using FluentAssertions;
    using Models;
    using Xunit;

    public class ManufacturerServiceTests : BaseTest
    {
        private const string SampleManufacturerName = "SampleManufacturerName";

        private readonly CarDealerDbContext dbContext;
        private readonly ManufacturerService manufacturerService;

        public ManufacturerServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.manufacturerService = new ManufacturerService(this.dbContext, Mapper.Instance);
        }

        [Fact]
        public async Task AllAsync_ShouldReturnCollectionWithCorrectModel()
        {
            // Arrange
            this.dbContext.Manufacturers.Add(new Manufacturer { Id = 1, Name = SampleManufacturerName });
            this.dbContext.Manufacturers.Add(new Manufacturer { Id = 2, Name = SampleManufacturerName });
            this.dbContext.Manufacturers.Add(new Manufacturer { Id = 3, Name = SampleManufacturerName });
            this.dbContext.SaveChanges();
            // Act
            var result = await this.manufacturerService.AllAsync();

            // Assert
            result
                .Should()
                .AllBeAssignableTo<ManufacturerConciseListModel>();

            result
                .Should()
                .HaveCount(3);
        }

        [Fact]
        public async Task AllAsync_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await this.manufacturerService.AllAsync();

            // Assert
            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task AllAsync_ShouldReturnOrderedByNameCollection()
        {
            // Arrange
            for (int i = 1; i <= 10; i++)
            {
                this.dbContext.Manufacturers.Add(new Manufacturer { Id = i, Name = $"Manufacturer_{i}" });
            }

            this.dbContext.SaveChanges();
            // Act
            var result = await this.manufacturerService.AllAsync();

            // Assert
            result
                .Should()
                .BeInAscendingOrder(x => x.Name);
        }

        [Fact]
        public async Task CreateAsync_WithoutName_ShouldReturnZeroAndNotInsertManufacturerInDatabase()
        {
            // Act
            var result = await this.manufacturerService.CreateAsync(null);

            // Assert
            result
                .Should()
                .Be(0);

            this.dbContext
                .Manufacturers
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidName_ShouldAddManufacturerToDatabaseAndReturnCorrectId()
        {
            // Act
            var result = await this.manufacturerService.CreateAsync(SampleManufacturerName);

            // Assert
            result
                .Should()
                .Be(1);

            this.dbContext
                .Manufacturers
                .Should()
                .HaveCount(1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalseAndNotDeleteManufacturerFromDatabase(int id)
        {
            // Arrange
            const int count = 4;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Manufacturers.Add(new Manufacturer { Id = i });
            }

            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Manufacturers
                .Should()
                .HaveCount(count);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrueAndDeleteManufacturerFromDatabase()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Manufacturers.Add(new Manufacturer { Id = id });

            this.dbContext.SaveChanges();
            // Act
            var result = await this.manufacturerService.DeleteAsync(id);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Manufacturers
                .Should()
                .HaveCount(0);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task GetForUpdateAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            for (int i = 1; i < 4; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetForUpdateAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetForUpdateAsync_WithValidId_ShouldReturnCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Add(new Manufacturer { Id = id });
            this.dbContext.SaveChanges();
            // Act
            var result = await this.manufacturerService.GetForUpdateAsync(id);

            // Assert
            result
                .Should()
                .BeAssignableTo<ManufacturerUpdateServiceModel>();
        }

        [Fact]
        public async Task GetForUpdateAsync_WithValidId_ShouldReturnCorrectEntity()
        {
            // Arrange
            const int count = 4;
            const int testedId = 1;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer
                {
                    Id = i,
                    Name = $"Manufacturer_{i}"
                });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetForUpdateAsync(testedId);

            // Assert
            result
                .Should()
                .Match(x => x.As<ManufacturerUpdateServiceModel>().Id == testedId)
                .And
                .Match(x => x.As<ManufacturerUpdateServiceModel>().Name == $"Manufacturer_{testedId}");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task GetDetailedAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            for (int i = 1; i < 4; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetDetailedAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetDetailedAsync_WithValidId_ShouldReturnCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Add(new Manufacturer { Id = id });
            this.dbContext.SaveChanges();
            // Act
            var result = await this.manufacturerService.GetDetailedAsync(id);

            // Assert
            result
                .Should()
                .BeAssignableTo<ManufacturerDetailsServiceModel>();
        }

        [Fact]
        public async Task GetDetailedAsync_WithValidId_ShouldReturnCorrectEntity()
        {
            // Arrange
            const int count = 4;
            const int testedId = 1;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer
                {
                    Id = i,
                    Name = $"Manufacturer_{i}"
                });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.GetDetailedAsync(testedId);

            // Assert
            result
                .Should()
                .Match(x => x.As<ManufacturerDetailsServiceModel>().Id == testedId)
                .And
                .Match(x => x.As<ManufacturerDetailsServiceModel>().Name == $"Manufacturer_{testedId}");
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
            const int count = 4;
            const string name = "some name";
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Add(new Manufacturer { Id = i });
            }
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.UpdateAsync(id, name);

            // Assert
            result
                .Should()
                .BeFalse();
            
            this.dbContext
                .Manufacturers
                .Should()
                .NotContain(x => x.Name == name);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task UpdateAsync_WithInvalidName_ShouldReturnFalse(string name)
        {
            // Arrange
            var testName = "BMW";
            this.dbContext.Manufacturers.Add(new Manufacturer {Id = 1, Name = testName});
            this.dbContext.SaveChanges();

            // Act
            var result = await this.manufacturerService.UpdateAsync(1, name);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_WithValidArguments_ShouldReturnTrueAndUpdateEntity()
        {
            // Arrange
            const int id = 1;
            var originalName = "Audi";
            var newName = "BMW";
            this.dbContext.Manufacturers.Add(new Manufacturer {Id = id, Name = originalName});
            this.dbContext.SaveChanges();
            // Act
            var result = await this.manufacturerService.UpdateAsync(id, newName);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Manufacturers
                .First()
                .Should()
                .Match(x => x.As<Manufacturer>().Name == newName);
        }
    }
}
