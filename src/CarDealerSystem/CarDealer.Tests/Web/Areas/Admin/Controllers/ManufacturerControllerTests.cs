namespace CarDealer.Tests.Web.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Services.Interfaces;
    using CarDealer.Services.Models;
    using CarDealer.Services.Models.Manufacturer;
    using CarDealer.Web;
    using CarDealer.Web.Areas.Admin.Controllers;
    using Common.Notifications;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using Xunit;

    public class ManufacturerControllerTests : BaseTest
    {
        private const string SampleManufacturerName = "BMW";
        private readonly ManufacturerController manufacturerController;

        private readonly Mock<IManufacturerService> manufacturers;

        public ManufacturerControllerTests()
        {
            this.manufacturers = new Mock<IManufacturerService>();

            this.manufacturerController = new ManufacturerController(this.manufacturers.Object);
        }

        [Fact]
        public void ManufacturerController_ShouldBeOnlyAccessedByAdministrators()
        {
            // Arrange
            var authorizeAttributes = this.manufacturerController
                .GetType()
                .GetCustomAttributes(true)
                .Where(attr => attr is AuthorizeAttribute)
                .Cast<AuthorizeAttribute>()
                .ToList();

            // Assert
            authorizeAttributes
                .Should()
                .Contain(attr => attr.Roles.Contains(WebConstants.SeniorAndAdminRoles));
        }

        [Fact]
        public async Task Index_ShouldReturnView()
        {
            this.manufacturers
                .Setup(m => m.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>());

            // Act
            var result = await this.manufacturerController.Index();

            // Assert
            result
                .Should()
                .BeAssignableTo<ViewResult>();
        }

        [Fact]
        public async Task Index_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.manufacturers
                .Setup(s => s.AllAsync())
                .ReturnsAsync(new List<ManufacturerConciseListModel>
                {
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                    new ManufacturerConciseListModel(),
                });

            // Act
            var result = await this.manufacturerController.Index() as ViewResult;
            var model = result?.ViewData.Model as IEnumerable<ManufacturerConciseListModel>;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .HaveCount(4)
                .And
                .AllBeAssignableTo<ManufacturerConciseListModel>();
        }

        [Fact]
        public async Task Details_WithInvalidId_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);
            this.manufacturers
                .Setup(m => m.GetDetailedAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerDetailsServiceModel));
            // Act
            var result = await this.manufacturerController.Details(10);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.manufacturerController.Index));
        }

        [Fact]
        public async Task Details_WithValidId_ShouldReturnViewWithCorrectEntity()
        {
            // Arrange
            const int id = 10;

            this.InitializeTempData(this.manufacturerController);

            this.manufacturers
                .Setup(m => m.GetDetailedAsync(It.IsAny<int>()))
                .ReturnsAsync(new ManufacturerDetailsServiceModel
                {
                    Id = id,
                    Name = SampleManufacturerName,
                    Models = new List<ModelConciseServiceModel>
                    {
                        new ModelConciseServiceModel(),
                        new ModelConciseServiceModel(),
                        new ModelConciseServiceModel(),
                        new ModelConciseServiceModel(),
                    }
                });
            // Act
            var result = await this.manufacturerController.Details(id) as ViewResult;
            var model = result?.ViewData.Model;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .Match<ManufacturerDetailsServiceModel>(m => m.Id == id)
                .And
                .Match<ManufacturerDetailsServiceModel>(m => m.Name == SampleManufacturerName)
                .And
                .Match<ManufacturerDetailsServiceModel>(m => m.Models.Count() == 4)
                .And
                .Match<ManufacturerDetailsServiceModel>(m => m.Models.All(mod => mod != null));
        }

        [Fact]
        public async Task Create_WithInvalidName_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);
            this.manufacturers
                .Setup(m => m.CreateAsync(It.IsAny<string>()))
                .ReturnsAsync(default(int));
            // Act
            await this.manufacturerController.Create(string.Empty);

            // Assert
            this.manufacturerController
                .TempData
                .Should()
                .Match<ITempDataDictionary>(td =>
                    td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));

            // Second way to check
            //this.manufacturerController
            //    .TempData
            //    .Should()
            //    .HaveCount(2);
        }

        [Fact]
        public async Task Create_WithValidName_ShouldSetSuccessNotification()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);
            this.manufacturers
                .Setup(m => m.CreateAsync(It.IsAny<string>()))
                .ReturnsAsync(1);
            // Act
            await this.manufacturerController.Create(SampleManufacturerName);

            // Assert
            this.manufacturerController
                .TempData
                .Should()
                .Match<ITempDataDictionary>(td =>
                    td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Success.ToString()));

            // Second way to check
            //this.manufacturerController
            //    .TempData
            //    .Should()
            //    .HaveCount(2);
        }

        [Fact]
        public async Task Create_WithValidName_ShouldRedirectToDetails()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);
            this.manufacturers
                .Setup(m => m.CreateAsync(It.IsAny<string>()))
                .ReturnsAsync(1);
            // Act
            var result = await this.manufacturerController.Create(SampleManufacturerName);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.manufacturerController.Details));
        }

        [Fact]
        public async Task Edit_WithInvalidId_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);
            this.manufacturers
                .Setup(m => m.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerUpdateServiceModel));

            // Act
            await this.manufacturerController.Edit(10);

            // Assert
            this.manufacturerController
                .TempData
                .Should()
                .Match<ITempDataDictionary>(td =>
                    td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));

        }

        [Fact]
        public async Task Edit_WithInvalidId_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);
            this.manufacturers
                .Setup(m => m.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerUpdateServiceModel));
            // Act
            var result = await this.manufacturerController.Edit(10);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.manufacturerController.Index));
        }

        [Fact]
        public async Task Edit_WithValidId_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.InitializeTempData(this.manufacturerController);

            var expectedModel = new ManufacturerUpdateServiceModel { Id = id, Name = SampleManufacturerName };

            this.manufacturers
                .Setup(m => m.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedModel);

            // Act
            var result = await this.manufacturerController.Edit(id) as ViewResult;
            var model = result?.ViewData.Model;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async Task Edit_Post_WithValidModel_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);

            this.manufacturers
                .Setup(m => m.UpdateAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.manufacturerController.Edit(new ManufacturerUpdateServiceModel
            { Id = 1, Name = SampleManufacturerName });

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.manufacturerController.Index));
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);

            this.manufacturers
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerUpdateServiceModel));

            // Act
            var result = await this.manufacturerController.Delete(1);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.manufacturerController.Index));
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);

            this.manufacturers
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(default(ManufacturerUpdateServiceModel));

            // Act
            await this.manufacturerController.Delete(1);
            var tempData = this.manufacturerController.TempData;

            // Assert
            tempData
                .Should()
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.InitializeTempData(this.manufacturerController);
            var expectedModel = new ManufacturerUpdateServiceModel
            {
                Id = id,
                Name = SampleManufacturerName
            };

            this.manufacturers
                .Setup(s => s.GetForUpdateAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedModel);

            // Act
            var result = await this.manufacturerController.Delete(id) as ViewResult;
            var model = result?.ViewData.Model;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async Task Delete_Post_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.manufacturerController);

            this.manufacturers
                .Setup(s => s.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.manufacturerController.Delete(new ManufacturerUpdateServiceModel());

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.manufacturerController.Index));
        }
    }
}
