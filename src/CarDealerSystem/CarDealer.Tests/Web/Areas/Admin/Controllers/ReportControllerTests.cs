namespace CarDealer.Tests.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using CarDealer.Services.Interfaces;
    using CarDealer.Services.Models.Report;
    using CarDealer.Web;
    using CarDealer.Web.Areas.Admin.Controllers;
    using CarDealer.Web.Areas.Admin.Models.Report;
    using Common.Notifications;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using Xunit;

    public class ReportControllerTests : BaseTest
    {
        private readonly DateTime SampleDate = new DateTime(2010, 10, 10);
        private readonly Mock<IReportService> reportService;
        private readonly Mock<IAdService> adService;

        private readonly ReportController reportController;

        public ReportControllerTests()
        {
            this.reportService = new Mock<IReportService>();
            this.adService = new Mock<IAdService>();

            this.reportController = new ReportController(this.adService.Object, this.reportService.Object, Mapper.Instance);
        }

        [Fact]
        public void LogController_ShouldBeAccessedOnlyByAdministrators()
        {
            // Arrange
            var authorizeAttributes = this.reportController
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
        public void Index__ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.InitializeTempData(this.reportController);
            var reports = this.GetCollectionOfReports(WebConstants.ReportsListPageSize * 4, true).AsQueryable();
            this.adService
                .Setup(l => l.GetAllReportedAds())
                .Returns(reports);

            // Act
            var result = this.reportController.Index() as ViewResult;
            var model = result?.ViewData.Model as AdReportListingViewModel;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<AdReportListingViewModel>(l => l.Results.PageIndex == 1)
                .And
                .Match<AdReportListingViewModel>(l => l.Results.TotalPages == 4)
                .And
                .Match<AdReportListingViewModel>(m => m.Results.HasNextPage)
                .And
                .Match<AdReportListingViewModel>(m => m.Results.HasPreviousPage == false);
        }

        [Fact]
        public void Index_WithPageIndex_ShouldReturnCorrectModelWithPage()
        {
            // Arrange
            const int expectedPageIndex = 3;
            this.InitializeTempData(this.reportController);

            var reports = this.GetCollectionOfReports(WebConstants.ReportsListPageSize * 4, true).AsQueryable();
            this.adService
                .Setup(a => a.GetAllReportedAds())
                .Returns(reports);

            // Act
            var result = this.reportController.Index(expectedPageIndex) as ViewResult;
            var model = result?.ViewData.Model as AdReportListingViewModel;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<AdReportListingViewModel>(a => a.Results.PageIndex == expectedPageIndex)
                .And
                .Match<AdReportListingViewModel>(a => a.Results.HasNextPage)
                .And
                .Match<AdReportListingViewModel>(a => a.Results.HasPreviousPage)
                .And
                .Match<AdReportListingViewModel>(a => a.Results.TotalPages == 4);
        }

        [Fact]
        public void Index_WithEmptyCollectionOfReports_ShouldSetNotificationAndReturnView()
        {

            // Arrange
            this.InitializeTempData(this.reportController);
            var reports = this.GetCollectionOfReports(0, true).AsQueryable();
            this.adService
                .Setup(l => l.GetAllReportedAds())
                .Returns(reports);

            // Act
            var result = this.reportController.Index() as ViewResult;

            // Assert
            result
                .Should()
                .NotBeNull();

            result?
                .TempData
                .Should()
                .Match<ITempDataDictionary>(td =>
                    td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Success.ToString()));
        }

        [Fact]
        public void Create_ShouldReturnView()
        {
            // Arrange
            this.InitializeTempData(this.reportController);
            // Act
            var result = this.reportController.Create(1);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .BeAssignableTo<ViewResult>();
        }

        [Fact]
        public async Task Submit_WithInvalidModel_ShouldRedirectToAdControllerDetailsAction()
        {
            // Arrange
            this.InitializeTempData(this.reportController);
            var model = new ReportViewModel()
            {
                Id = 1,
                Description = "asd"
            };
            this.reportService
                .Setup(r=> r.CreateAsync(new ReportServiceModel()))
                .ReturnsAsync(false);

            // Act
            var result = await this.reportController.Submit(model);

            // Assert
            result
                .Should()
                .Match<RedirectToActionResult>(r =>
                    r.ActionName == "Details" && r.ControllerName == "Ad");
        }

        [Fact]
        public async Task Submit_WithInvalidModel_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.reportController);
            var model = new ReportViewModel()
            {
                Id = 1,
                Description = "asd"
            };
            this.reportService
                .Setup(r => r.CreateAsync(new ReportServiceModel()))
                .ReturnsAsync(false);

            // Act
            var result = await this.reportController.Submit(model) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .Match<ITempDataDictionary>(td=> td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task Submit_WithValidModel_ShouldSetSuccessNotification()
        {
            // Arrange
            this.InitializeTempData(this.reportController);
            var model = new ReportViewModel()
            {
                Id = 1,
                Description = "asd"
            };
            this.reportService
                .Setup(r => r.CreateAsync(new ReportServiceModel()))
                .ReturnsAsync(true);

            // Act
            var result = await this.reportController.Submit(model) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .Match<ITempDataDictionary>(td => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Success.ToString()));
        }

        [Fact]
        public async Task Update_WithValidId_ShouldSetSuccessNotification()
        {
            // Arrange
            this.InitializeTempData(this.reportController);
            this.adService
                .Setup(a => a.UpdateReportedAdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var result = await this.reportController.Update(1) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .Match<ITempDataDictionary>(td =>
                    td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Success.ToString()));
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.reportController);
            this.adService
                .Setup(a => a.UpdateReportedAdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await this.reportController.Update(1) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .Match<ITempDataDictionary>(td =>
                    td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task Update_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.reportController);
            
            // Act
            var result = await this.reportController.Update(1);

            // Arrange
            result
                .Should()
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.reportController.Index));
        }

        private IEnumerable<ReportListingServiceModel> GetCollectionOfReports(int count, bool randomDate = false)
        {
            var random = new Random();

            var reports = new List<ReportListingServiceModel>();
            for (int i = 0; i < count; i++)
            {
                var report = new ReportListingServiceModel
                {
                    Description = $"Sample Description_{i}",
                    CreationDate = randomDate ? this.SampleDate.AddDays(random.Next(1, 1000)) : this.SampleDate,
                };

                reports.Add(report);
            }

            return reports;
        }
    }
}
