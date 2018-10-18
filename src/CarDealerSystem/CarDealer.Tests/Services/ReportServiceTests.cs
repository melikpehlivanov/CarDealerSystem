namespace CarDealer.Tests.Services
{
    using System.Threading.Tasks;
    using CarDealer.Services.Implementations.Report;
    using CarDealer.Services.Models.Report;
    using Data;
    using FluentAssertions;
    using Models;
    using Xunit;

    public class ReportServiceTests : BaseTest
    {
        private readonly CarDealerDbContext dbContext;
        private readonly ReportService reportService;

        public ReportServiceTests()
        {
            this.dbContext = this.DatabaseInstance;
            this.reportService = new ReportService(this.dbContext);
        }

        [Fact]
        public async Task CreateAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            this.dbContext.Ads.Add(new Ad {Id = 1});
            this.dbContext.SaveChanges();
            var model = new ReportServiceModel
            {
                Id = 1,
                Description = "SampleDescription"
            };
            // Act
            var result = await this.reportService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task CreateAsync_WithInvalidModel_ShouldReturnFalse()
        {
            // Arrange
            this.dbContext.Ads.Add(new Ad { Id = 1 });
            this.dbContext.SaveChanges();
            var model = new ReportServiceModel();
            // Act
            var result = await this.reportService.CreateAsync(model);

            // Assert
            result
                .Should()
                .BeFalse();
        }
    }
}
