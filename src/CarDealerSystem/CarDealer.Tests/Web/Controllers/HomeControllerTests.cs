namespace CarDealer.Tests.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Web.Controllers;
    using CarDealer.Web.Infrastructure.Collections.Interfaces;
    using CarDealer.Web.Models;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Moq;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly HomeController controller;
        private readonly Mock<ICache> cache;

        public HomeControllerTests()
        {
            this.cache = new Mock<ICache>();
            this.controller = new HomeController(this.cache.Object);
        }

        [Fact]
        public async Task Index_ShouldReturnView()
        {
            // Arrange
            this.cache
                .Setup(s => s.GetAllManufacturersAsync())
                .ReturnsAsync(new List<SelectListItem>
                {
                    new SelectListItem(),
                    new SelectListItem(),
                    new SelectListItem(),
                    new SelectListItem(),
                });

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
            this.cache
                .Setup(s => s.GetAllManufacturersAsync())
                .ReturnsAsync(new List<SelectListItem>
                {
                    new SelectListItem(),
                    new SelectListItem(),
                    new SelectListItem(),
                    new SelectListItem(),
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
