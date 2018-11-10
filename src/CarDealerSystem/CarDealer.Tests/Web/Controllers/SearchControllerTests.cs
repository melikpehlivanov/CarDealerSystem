namespace CarDealer.Tests.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Services.Interfaces;
    using CarDealer.Services.Models.Manufacturer;
    using CarDealer.Services.Models.Vehicle;
    using CarDealer.Web;
    using CarDealer.Web.Controllers;
    using CarDealer.Web.Infrastructure.Collections.Interfaces;
    using CarDealer.Web.Models;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Models.BasicTypes;
    using Moq;
    using Xunit;

    public class SearchControllerTests
    {
        private readonly SearchController searchController;
        
        private readonly Mock<ICache> cache;
        private readonly Mock<IVehicleService> vehicles;
        private readonly Mock<IManufacturerService> manufacturers;
        private readonly Mock<IVehicleElementService> vehicleElements;

        public SearchControllerTests()
        {
            this.cache = new Mock<ICache>();
            this.vehicles = new Mock<IVehicleService>();
            this.manufacturers = new Mock<IManufacturerService>();
            this.vehicleElements = new Mock<IVehicleElementService>();

            this.searchController = new SearchController(this.vehicles.Object, this.cache.Object);
        }

        [Fact]
        public void Index_ShouldRedirectToResultWithDefaultRouteValues()
        {
            // Act
            var result = this.searchController.Index() as RedirectToActionResult;
            var actionName = result?.ActionName;
            var routeValues = result?.RouteValues;

            // Assert
            result
                .Should()
                .NotBeNull();

            actionName
                .Should()
                .Be(nameof(this.searchController.Result));

            routeValues
                .Should()
                .ContainKey("manufacturerId")
                .WhichValue
                .Should()
                .Be(default(int));

            routeValues
                .Should()
                .ContainKey("modelName")
                .WhichValue
                .Should()
                .Be(string.Empty);
        }

        [Fact]
        public async Task Result_ShouldReturnView()
        {
            // Arrange
            this.SetUpDependenciesWithDefaultValues();

            // Act
            var result = await this.searchController.Result(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>());

            // Assert
            result
                .Should()
                .BeAssignableTo<ViewResult>();
        }

        [Fact]
        public async Task Result_ShouldReturnViewWithCorrectPaging()
        {
            // Arrange
            const int pageIndex = 2;
            this.SetUpDependenciesWithDefaultValues();

            this.vehicles
                .Setup(v => v.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(this.GetCollectionOf<VehicleSearchServiceModel>(50).AsQueryable());

            // Act
            var result = await this.searchController.Result(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), pageIndex);
            var viewModel = (result as ViewResult)?.ViewData?.Model;

            // Assert
            viewModel
                .Should()
                .NotBeNull()
                .And
                .BeAssignableTo<SearchViewModel>()
                .And
                .Match<SearchViewModel>(m => m.Results.PageIndex == pageIndex)
                .And
                .Match<SearchViewModel>(m => m.Results.Count() == WebConstants.SearchResultsPageSize);
        }

        #region privateMethods

        private IEnumerable<TModel> GetCollectionOf<TModel>(int count)
        {
            var modelType = typeof(TModel);
            var collection = new List<TModel>();

            for (int i = 0; i < count; i++)
            {
                collection.Add((TModel)Activator.CreateInstance(modelType));
            }

            return collection;
        }

        private void SetUpDependenciesWithDefaultValues()
        {
            this.vehicles
                .Setup(v => v.Get(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>(), It.IsAny<decimal>()))
                .Returns(new List<VehicleSearchServiceModel>().AsQueryable());

            this.manufacturers
                .Setup(m => m.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>());

            this.vehicleElements
                .Setup(ve => ve.GetTransmissionTypesAsync())
                .ReturnsAsync(new List<TransmissionType>());

            this.vehicleElements
                .Setup(ve => ve.GetFeaturesAsync())
                .ReturnsAsync(new List<Feature>());

            this.vehicleElements
                .Setup(ve => ve.GetFuelTypesAsync())
                .ReturnsAsync(new List<FuelType>());
        }

        #endregion
    }
}
