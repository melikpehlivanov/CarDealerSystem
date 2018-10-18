namespace CarDealer.Tests.Services
{
    using System.Collections.Generic;
    using AutoMapper;
    using CarDealer.Services.Implementations.User;
    using CarDealer.Services.Models.User;
    using Data;
    using FluentAssertions;
    using Models;
    using Xunit;

    public class UserServiceTests : BaseTest
    {
        private readonly CarDealerDbContext dbContext;
        private readonly UserService userService;

        public UserServiceTests()
        {
            this.dbContext = this.DatabaseInstance;
            this.userService = new UserService(this.dbContext, Mapper.Instance);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void GetAll_ShouldReturnCorrectCountOfUsers(int count)
        {
            // Arrange
            this.SeedUsers(count);
            // Act
            var result = this.userService.GetAll();

            // Assert
            result
                .Should()
                .HaveCount(count);
        }

        [Fact]
        public void GetAll_ShouldReturnCorrectModel()
        {
            // Arrange
            const int count = 4;
            this.SeedUsers(count);

            // Act
            var result = this.userService.GetAll();
            
            // Assert
            result
                .Should()
                .AllBeAssignableTo<UserListingServiceModel>();
        }

        [Fact]
        public void GetAll_ShouldReturnOrderedCollection()
        {
            // Arrange
            const int count = 4;
            this.SeedUsers(count);

            // Act
            var result = this.userService.GetAll();

            // Assert
            result
                .Should()
                .BeInAscendingOrder(u=> u.Email);
        }

        private void SeedUsers(int count)
        {
            var users = new List<User>();
            for (int i = 1; i <= count; i++)
            {
                users.Add(new User{Id = i.ToString(), Email = $"User_{i}@test.com"});
            }
            this.dbContext.Users.AddRange(users);
            this.dbContext.SaveChanges();
        }
    }
}
