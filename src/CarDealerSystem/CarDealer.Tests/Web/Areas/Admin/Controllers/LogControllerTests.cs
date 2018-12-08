namespace CarDealer.Tests.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Services.Interfaces;
    using CarDealer.Services.Models.Logs;
    using CarDealer.Web;
    using CarDealer.Web.Areas.Admin.Controllers;
    using CarDealer.Web.Areas.Admin.Models.Logs;
    using CarDealer.Web.Infrastructure.Collections;
    using Common.Notifications;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using Xunit;

    public class LogControllerTests : BaseTest
    {
        private readonly DateTime SampleDate = new DateTime(2010, 10, 10);
        private readonly LogsController logController;
        private readonly Mock<ILogService> logs;

        public LogControllerTests()
        {
            this.logs = new Mock<ILogService>();

            this.logController = new LogsController(this.logs.Object);
        }

        [Fact]
        public void LogController_ShouldBeAccessedOnlyByAdministrators()
        {
            // Arrange
            var authorizeAttributes = this.logController
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
        public void Index_WithNoSearchTerms_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            this.InitializeTempData(this.logController);

            var logs = this.GetCollectionOfLogs(WebConstants.LogsListPageSize * 4, true).AsQueryable();
            this.logs
                .Setup(l => l.GetAll())
                .Returns(logs);

            // Act
            var result = this.logController.Index(null) as ViewResult;
            var model = result?.ViewData.Model as UserActivityLogListViewModel;
            var modelLogs = model?.Logs;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserActivityLogListViewModel>(l=> l.Logs.PageIndex == 1)
                .And
                .Match<UserActivityLogListViewModel>(l=> l.Logs.TotalPages == 4)
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.HasNextPage)
                .And
                .Match<UserActivityLogListViewModel>(m => m.Logs.HasPreviousPage == false);
            
            modelLogs
                .Should()
                .NotBeNull()
                .And
                .BeInDescendingOrder(l=> l.DateTime);
        }

        [Fact]
        public void Index_WithSearchTerms_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const string searchTerm = "User_10";

            this.InitializeTempData(this.logController);
            var getLogs = this.GetCollectionOfLogs(WebConstants.LogsListPageSize * 4, true).AsQueryable();
            this.logs
                .Setup(l => l.GetAll())
                .Returns(getLogs);

            // Act
            var result = this.logController.Index(searchTerm) as ViewResult;
            var model = result?.ViewData.Model as UserActivityLogListViewModel;
            var modelLogs = model?.Logs;

            // Assert
            result
                .Should()
                .NotBeNull();

            modelLogs
                .Should()
                .NotBeNull()
                .And
                .BeInDescendingOrder(l => l.DateTime)
                .And
                .Match<PaginatedList<UserActivityLogConciseServiceModel>>(l=> l.All(u=> u.UserEmail.Contains(searchTerm)));
        }

        [Fact]
        public void Index_WithPageIndex_ShouldReturnCorrectModelWithPage()
        {
            // Arrange
            const int expectedPageIndex = 3;
            this.InitializeTempData(this.logController);

            var getLogs = this.GetCollectionOfLogs(WebConstants.LogsListPageSize * 4, true).AsQueryable();
            this.logs
                .Setup(l => l.GetAll())
                .Returns(getLogs);

            // Act
            var result = this.logController.Index(null, expectedPageIndex) as ViewResult;
            var model = result?.ViewData.Model as UserActivityLogListViewModel;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserActivityLogListViewModel>(l => l.Logs.PageIndex == expectedPageIndex)
                .And
                .Match<UserActivityLogListViewModel>(l => l.Logs.TotalPages == 4)
                .And
                .Match<UserActivityLogListViewModel>(l => l.Logs.HasNextPage)
                .And
                .Match<UserActivityLogListViewModel>(l => l.Logs.HasPreviousPage);
        }

        [Fact]
        public async Task Details_WithInvalidId_ShouldRedirectToIndex()
        {
            // Arrange
            this.InitializeTempData(this.logController);

            this.logs
                .Setup(s => s.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(default(UserActivityLogDetailsServiceModel));

            // Act
            var result = await this.logController.Details(1);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.logController.Index));
        }

        [Fact]
        public async Task Details_WithInvalidId_ShouldSetErrorNotification()
        {
            // Arrange
            this.InitializeTempData(this.logController);

            this.logs
                .Setup(s => s.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(default(UserActivityLogDetailsServiceModel));

            // Act
            await this.logController.Details(1);
            var tempData = this.logController.TempData;

            // Assert
            tempData
                .Should()
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task Details_WithValidId_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int id = 1;
            this.InitializeTempData(this.logController);
            var expectedModel = new UserActivityLogDetailsServiceModel
            {
                Id = id,
                DateTime = new DateTime(2010, 10, 10),
                UserEmail = "sometest@test.com"
            };

            this.logs
                .Setup(s => s.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedModel);

            // Act
            var result = await this.logController.Details(id) as ViewResult;
            var actionModel = result?.ViewData.Model;

            // Assert
            result
                .Should()
                .NotBeNull();

            actionModel
                .Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(expectedModel);
        }

        private IEnumerable<UserActivityLogConciseServiceModel> GetCollectionOfLogs(int count, bool randomDate = false)
        {
            var random = new Random();
            var httpMethods = new[] { "GET", "POST" };
            var UserEmailTemplate = "User_{0}";

            var logs = new List<UserActivityLogConciseServiceModel>();
            for (int i = 0; i < count; i++)
            {
                var log = new UserActivityLogConciseServiceModel
                {
                    DateTime = randomDate ? this.SampleDate.AddDays(random.Next(1, 1000)) : this.SampleDate,
                    UserEmail = string.Format(UserEmailTemplate, i),
                    HttpMethod = httpMethods[random.Next(0, httpMethods.Length)],
                };

                logs.Add(log);
            }

            return logs.OrderByDescending(l => l.DateTime);
        }
    }
}
