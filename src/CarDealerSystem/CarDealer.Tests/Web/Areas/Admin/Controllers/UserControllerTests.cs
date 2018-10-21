namespace CarDealer.Tests.Web.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Services.Interfaces;
    using CarDealer.Services.Models.User;
    using CarDealer.Web;
    using CarDealer.Web.Areas.Admin.Controllers;
    using CarDealer.Web.Areas.Admin.Models.User;
    using Common.Notifications;
    using FluentAssertions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Models;
    using Moq;
    using Xunit;

    public class UserControllerTests : BaseTest
    {
        private const string SampleUserEmail = "Sample@Sample.com";
        private const int UsersCount = 40;
        private readonly Mock<UserManager<User>> userManager;
        private readonly Mock<RoleManager<IdentityRole>> roleManager;
        private readonly Mock<IUserService> userService;

        private readonly UserController userController;

        public UserControllerTests()
        {
            this.userManager = MockGenerator.UserManagerMock;
            this.roleManager = MockGenerator.RoleManagerMock;
            this.userService = new Mock<IUserService>();

            this.userController = new UserController(this.roleManager.Object, this.userManager.Object, this.userService.Object);
        }

        [Fact]
        public void UsersController_ShouldBeAccessedOnlyByAdministrators()
        {
            // Arrange
            var authorizeAttributes = this.userController
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
        public async Task Index_WithSearchTerm_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int usersCount = 30;
            const string searchTerm = "User_1";
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);

            // Act
            var result = await this.userController.Index(searchTerm) as ViewResult;
            var model = result?.ViewData.Model as UserListingViewModel;

            // Assert
            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserListingViewModel>(m => m.Users
                    .All(u => u.Email.ToLower().Contains(searchTerm.ToLower())));
        }

        [Fact]
        public async Task Index_WithoutSearchTerm_ShouldReturnModelWithCorrectPageIndex()
        {
            // Arrange
            const int pageIndex = 2;
            const int usersCount = WebConstants.UsersListPageSize * 4;
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);

            // Act
            var result = await this.userController.Index(null, pageIndex) as ViewResult;
            var model = result?.ViewData.Model as UserListingViewModel;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserListingViewModel>(m => m.Users.PageIndex == pageIndex);
        }

        [Fact]
        public async Task Index_WithoutSearchTerm_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int usersCount = 30;
            var users = this.GetCollectionOfUsers(usersCount).AsQueryable();
            this.SetupDependencies(users);

            // Act
            var result = await this.userController.Index(null) as ViewResult;
            var model = result?.ViewData.Model as UserListingViewModel;

            // Assert
            result
                .Should()
                .NotBeNull();

            model
                .Should()
                .NotBeNull()
                .And
                .Match<UserListingViewModel>(m => m.Users
                    .Count() == WebConstants.UsersListPageSize)
                .And
                .Match<UserListingViewModel>(m => m.Users
                    .All(u => u != null));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddToRole_WithoutRole_ShouldReturnToIndex(string invalidRole)
        {
            // Arrange
            this.InitializeTempData(this.userController);

            // Act
            var result = await this.userController.AddToRole(SampleUserEmail, invalidRole) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.userController.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddToRole_WithoutRole_ShouldSetErrorNotification(string invalidRole)
        {
            // Arrange
            this.InitializeTempData(this.userController);

            // Act
            var result = await this.userController.AddToRole(SampleUserEmail, invalidRole) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddToRole_WithoutEmail_ShouldReturnToIndex(string invalidEmail)
        {
            // Arrange
            this.InitializeTempData(this.userController);

            // Act
            var result = await this.userController.AddToRole(invalidEmail, WebConstants.AdministratorRole) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.userController.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddToRole_WithoutEmail_ShouldSetErrorNotification(string invalidEmail)
        {
            // Arrange
            this.InitializeTempData(this.userController);

            // Act
            var result = await this.userController.AddToRole(invalidEmail, WebConstants.AdministratorRole) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RemoveFromRole_WithoutRole_ShouldReturnToIndex(string invalidRole)
        {
            // Arrange
            this.InitializeTempData(this.userController);

            // Act
            var result = await this.userController.RemoveFromRole(SampleUserEmail, invalidRole) as RedirectToActionResult;

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.userController.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RemoveFromRole_WithoutRole_ShouldSetErrorNotification(string invalidRole)
        {
            // Arrange
            this.InitializeTempData(this.userController);

            // Act
            var result = await this.userController.RemoveFromRole(SampleUserEmail, invalidRole) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RemoveFromRole_WithoutEmail_ShouldReturnToIndex(string invalidEmail)
        {
            // Arrange
            this.InitializeTempData(this.userController);

            // Act
            var result = await this.userController.RemoveFromRole(invalidEmail, WebConstants.AdministratorRole);

            // Assert
            result
                .Should()
                .NotBeNull()
                .And
                .Match<RedirectToActionResult>(r => r.ActionName == nameof(this.userController.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task RemoveFromRole_WithoutEmail_ShouldSetErrorNotification(string invalidEmail)
        {
            // Arrange
            this.InitializeTempData(this.userController);

            // Act
            var result = await this.userController.RemoveFromRole(invalidEmail, WebConstants.AdministratorRole) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task RemoveFromRole_WhenNotSuccess_ShouldSetErrorNotification()
        {
            // Arrange
            this.AddClaimsPrincipal(this.userController, "SomeUserName");
            this.InitializeTempData(this.userController);
            var users = this.GetCollectionOfUsers(UsersCount).AsQueryable();
            this.SetupDependencies(users);

            this.userManager
                .Setup(um => um.GetUserAsync(this.userController.User))
                .ReturnsAsync(new User { Email = "Some@Email.com" });

            this.userManager
                .Setup(um => um.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));


            // Act
            var result = await this.userController.RemoveFromRole(SampleUserEmail, WebConstants.AdministratorRole) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Error.ToString()));
        }

        [Fact]
        public async Task RemoveFromRole_WhenSuccess_ShouldSetSuccessNotification()
        {
            // Arrange
            this.AddClaimsPrincipal(this.userController, "SomeUserName");
            this.InitializeTempData(this.userController);
            var users = this.GetCollectionOfUsers(UsersCount).AsQueryable();
            this.SetupDependencies(users);

            this.userManager
                .Setup(um => um.GetUserAsync(this.userController.User))
                .ReturnsAsync(new User { Email = "Some@Email.com" });

            this.userManager
                .Setup(um => um.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);


            // Act
            var result = await this.userController.RemoveFromRole(SampleUserEmail, WebConstants.AdministratorRole) as ViewResult;

            // Assert
            result?
                .TempData
                .Should()
                .NotBeNull()
                .And
                .Match<ITempDataDictionary>(td
                    => td[NotificationConstants.NotificationTypeKey].Equals(NotificationType.Success.ToString()));
        }

        #region privateMethods

        private void SetupDependencies(IQueryable<UserListingServiceModel> users)
        {
            var roles = new[]
            {
                new IdentityRole(WebConstants.AdministratorRole),
                new IdentityRole(WebConstants.SeniorAdministratorRole),
            };

            this.userService
                .Setup(s => s.GetAll())
                .Returns(users);

            this.roleManager
                .Setup(rm => rm.Roles)
                .Returns(roles.AsQueryable());

            this.userManager
                .Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            this.userManager
                .Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(new List<string> { WebConstants.AdministratorRole });
        }

        private IEnumerable<UserListingServiceModel> GetCollectionOfUsers(int count)
        {
            var users = new List<UserListingServiceModel>();

            for (int i = 0; i < count; i++)
            {
                var user = new UserListingServiceModel
                {
                    Email = string.Format($"User_{i}"),
                };

                users.Add(user);
            }

            return users;
        }

        #endregion

    }
}
