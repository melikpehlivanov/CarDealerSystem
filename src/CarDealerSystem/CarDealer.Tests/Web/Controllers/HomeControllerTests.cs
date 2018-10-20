namespace CarDealer.Tests.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Services.Interfaces;
    using CarDealer.Services.Models.Manufacturer;
    using CarDealer.Web.Controllers;
    using CarDealer.Web.Models;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly HomeController controller;
        private readonly Mock<IManufacturerService> manufacturerService;

        public HomeControllerTests()
        {
            this.manufacturerService = new Mock<IManufacturerService>();
            this.controller = new HomeController(this.manufacturerService.Object);
        }

        [Fact]
        public async Task Index_ShouldReturnView()
        {
            // Arrange
            this.manufacturerService
                .Setup(s => s.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>());

            // Act
            var result = await this.controller.Index();

            // Assert
            result
                .Should()
                .BeAssignableTo<ViewResult>();
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.manufacturerService
                .Setup(s => s.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>
                {
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                });

            // Act
            var result = await this.controller.Index() as ViewResult;
            var model = result?.ViewData.Model;

            // Assert
            model
                .Should()
                .BeAssignableTo<IndexViewModel>()
                .And
                .Match<IndexViewModel>(m => m.AllManufacturers.Count() == 4)
                .And
                .Match<IndexViewModel>(m => m.AllManufacturers.All(manufacturer => manufacturer != null));
        }

        [Fact]
        public void About_ShouldReturnView()
        {
            // Act
            var result = this.controller.About();

            // Assert
            result
                .Should()
                .BeAssignableTo<ViewResult>();
        }

        [Fact]
        public void Contact_ShouldReturnView()
        {
            // Act
            var result = this.controller.Contact();

            // Assert
            result
                .Should()
                .BeAssignableTo<ViewResult>();
        }
    }
}
