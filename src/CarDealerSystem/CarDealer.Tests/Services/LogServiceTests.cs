namespace CarDealer.Tests.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using CarDealer.Services.Implementations.Logs;
    using CarDealer.Services.Models.Logs;
    using Data;
    using FluentAssertions;
    using Models;
    using Xunit;

    public class LogServiceTests : BaseTest
    {
        private const string ControllerName = "SomeController";
        private const string ActionName = "SomeAction";
        private const string HttpMethod = "GET";
        private const string UserEmail = "test@test.com";

        private readonly CarDealerDbContext dbContext;
        private readonly LogService logService;

        public LogServiceTests()
        {
            this.dbContext = base.DatabaseInstance;
            this.logService = new LogService(this.dbContext, Mapper.Instance);
        }

        [Fact]
        public void CreateUserActivityLog_WithValidData_ShouldAddLogToDatabasewithCorrectValues()
        {
            // Arrange
            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2010, 10, 10),
                UserEmail = UserEmail,
                ActionName = ActionName,
                ControllerName = ControllerName,
                HttpMethod = HttpMethod
            };

            // Act
            this.logService.CreateUserActivityLog(model);

            var logs = this.dbContext
                .Logs
                .FirstOrDefault();

            // Assert
            this.dbContext
                .Logs
                .Should()
                .HaveCount(1);

            logs
                .Should()
                .BeAssignableTo<UserActivityLog>();

            logs
                .Should()
                .Match(x => x.As<UserActivityLog>().UserEmail == UserEmail);

            logs
                .Should()
                .Match(x => x.As<UserActivityLog>().ControllerName == ControllerName);

            logs
                .Should()
                .Match(x => x.As<UserActivityLog>().ActionName == ActionName);

            logs
                .Should()
                .Match(x => x.As<UserActivityLog>() .HttpMethod == HttpMethod);
        }

        [Fact]
        public void CreateUserActivityLog_WithMissingUserEmail_ShouldNotInsertEntityToDatabase()
        {
            // Arrange
            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2000, 1, 1),
                ActionName = ActionName,
                ControllerName = ControllerName,
                HttpMethod = HttpMethod
            };

            // Act
            var result = this.logService.CreateUserActivityLog(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Logs
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void CreateUserActivityLog_WithMissingController_ShouldNotInsertEntityToDatabase()
        {
            // Arrange
            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2010, 10, 10),
                UserEmail = UserEmail,
                ActionName = ActionName,
                HttpMethod = HttpMethod
            };

            // Act
            var result = this.logService.CreateUserActivityLog(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Logs
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void CreateUserActivityLog_WithMissingAction_ShouldNotInsertEntityToDatabase()
        {
            // Arrange
            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2010, 10, 10),
                UserEmail = UserEmail,
                ControllerName = ControllerName,
                HttpMethod = HttpMethod
            };

            // Act
            var result = this.logService.CreateUserActivityLog(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Logs
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void CreateUserActivityLog_WithMissingHttpMethod_ShouldNotInsertEntityToDatabase()
        {
            // Arrange
            var model = new UserActivityLogCreateModel
            {
                DateTime = new DateTime(2000, 1, 1),
                UserEmail = UserEmail,
                ActionName = ActionName,
                ControllerName = ControllerName
            };

            // Act
            var result = this.logService.CreateUserActivityLog(model);

            // Assert
            result
                .Should()
                .BeFalse();

            this.dbContext
                .Logs
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GetAll_ShouldReturnQueryWithValidModel()
        {
            // Arrange
            this.dbContext.Logs.Add(new UserActivityLog());
            this.dbContext.Logs.Add(new UserActivityLog());
            this.dbContext.SaveChanges();

            // Act
            var result = this.logService.GetAll();

            // Assert
            result
                .Should()
                .BeAssignableTo<IQueryable<UserActivityLogConciseServiceModel>>();
        }

        [Fact]
        public void GetAll_ShouldOrderEntitiesByDateDescending()
        {
            // Arrange
            var date = new DateTime(2010, 10, 10);
            const int count = 4;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Logs.Add(new UserActivityLog
                {
                    Id = i,
                    DateTime = date.AddDays(i)
                });
            }

            ;
            this.dbContext.SaveChanges();

            // Act
            var result = this.logService.GetAll();

            // Assert
            result
                .Should()
                .HaveCount(count);

            result
                .Should()
                .BeInDescendingOrder(x => x.DateTime);
        }

        [Fact]
        public void GetAll_ShouldReturnCorrectCountOfEntities()
        {
            // Arrange
            const int count = 4;
            for (int i = 1; i <= count; i++)
            {
                this.dbContext.Logs.Add(new UserActivityLog());
            }

            ;
            this.dbContext.SaveChanges();

            // Act
            var result = this.logService.GetAll();

            // Assert
            result
                .Should()
                .HaveCount(count);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public async Task GetAsync_WithInvalidId_ShouldReturnNull(int id)
        {
            // Arrange
            for (int i = 1; i < 4; i++)
            {
                this.dbContext.Add(new UserActivityLog { Id = i });
            }

            this.dbContext.SaveChanges();

            // Act
            var result = await this.logService.GetAsync(id);

            // Assert
            result
                .Should()
                .BeNull();
        }
        
        [Fact]
        public async Task GetAsync_WithValidId_ShouldReturnCorrectEntity()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Add(new UserActivityLog { Id = id });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.logService.GetAsync(id);

            // Assert
            result
                .Should()
                .Match(x => x.As<UserActivityLogDetailsServiceModel>().Id == id);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.dbContext.Add(new UserActivityLog { Id = id });
            this.dbContext.SaveChanges();

            // Act
            var result = await this.logService.GetAsync(id);

            // Assert
            result
                .Should()
                .BeAssignableTo<UserActivityLogDetailsServiceModel>();
        }
    }
}