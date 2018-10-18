namespace CarDealer.Tests.Services
{
    using System.Threading.Tasks;
    using CarDealer.Services.Implementations.Picture;
    using Data;
    using FluentAssertions;
    using Models;
    using Xunit;

    public class PictureServiceTests : BaseTest
    {
        private const string SamplePicturePath = "SamplePicturePath";

        private readonly CarDealerDbContext dbContext;
        private readonly PictureService pictureService;

        public PictureServiceTests()
        {
            this.dbContext = this.DatabaseInstance;
            this.pictureService = new PictureService(this.dbContext);
        }

        [Fact]
        public async Task CreateAsync_WithValidArguments_ShouldReturnTrue()
        {
            // Arrange
            this.dbContext.Vehicles.Add(new Vehicle {Id = 1, IsDeleted = false});
            this.dbContext.SaveChanges();
            
            // Act
            var result = await this.pictureService.CreateAsync(SamplePicturePath, 1);

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
        public async Task CreateAsync_WithInvalidVehicleId_ShouldReturnFalse(int id)
        {
            // Arrange
            const int count = 4;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Vehicles.Add(new Vehicle {Id = i, IsDeleted = false});
            }

            this.dbContext.SaveChanges();
            // Act
            var result = await this.pictureService.CreateAsync(SamplePicturePath, id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidPicturePath_ShouldReturnFalse()
        {
            // Arrange
            this.dbContext.Vehicles.Add(new Vehicle {Id = 1, IsDeleted = false});
            this.dbContext.SaveChanges();
            // Act
            var result = await this.pictureService.CreateAsync(null, 1);

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
        public async Task RemoveAsync_WithInvalidId_ShouldReturnFalse(int id)
        {
            // Arrange
            const int count = 4;
            SeedPictures(count);

            // Act
            var result = await this.pictureService.RemoveAsync(id);

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public async Task RemoveAsync_WithValidId_ShouldReturnTrueAndRemovePictureFromDatabase()
        {
            // Arrange
            const int count = 4;
            SeedPictures(count);
            // Act
            var result = await this.pictureService.RemoveAsync(1);

            // Assert
            result
                .Should()
                .BeTrue();

            this.dbContext
                .Pictures
                .Should()
                .HaveCount(count - 1);
        }

        #region privateMethods

        private void SeedPictures(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Pictures.Add(new Picture { Id = i, Path = SamplePicturePath, VehicleId = i });
            }

            this.dbContext.SaveChanges();
        }

        #endregion
    }
}
