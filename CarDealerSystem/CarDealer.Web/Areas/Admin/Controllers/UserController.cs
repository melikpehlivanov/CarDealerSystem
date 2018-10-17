namespace CarDealer.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Models;
    using Common.Notifications;
    using Infrastructure.Collections;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.User;
    using Services.Interfaces;
    using Services.Models;
    using Services.Models.User;

    public class UserController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IUserService userService;

        public UserController(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            IUserService userService)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.userService = userService;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1)
        {
            page = Math.Max(1, page);
            var allUsers = this.userService.GetAll();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allUsers = allUsers.Where(u => u.Email.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalPages = (int)(Math.Ceiling(allUsers.Count() / (double)WebConstants.UsersListPageSize));
            page = Math.Min(page, Math.Max(1, totalPages));

            var usersToShow = allUsers
                .Skip((page - 1) * WebConstants.UsersListPageSize)
                .Take(WebConstants.UsersListPageSize)
                .ToList();

            var allRoles = this.roleManager.Roles.Select(r => r.Name).ToList();

            foreach (var userModel in usersToShow)
            {
                var user = await this.userManager.FindByEmailAsync(userModel.Email);
                var userRoles = await this.userManager.GetRolesAsync(user);

                userModel.CurrentRoles = userRoles;
                userModel.NonCurrentRoles = allRoles.Except(userRoles).ToList();
            }

            var model = new UserListingViewModel
            {
                SearchTerm = searchTerm,
                Users = new PaginatedList<UserListingServiceModel>(usersToShow, page, totalPages),
            };

            return View(model);
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> AddToRole(string userEmail, string role)
        {
            if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(userEmail))
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await this.userManager.FindByEmailAsync(userEmail);
            if (this.User.IsInRole(WebConstants.AdministratorRole) && role == WebConstants.SeniorAdministratorRole)
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.AdminCannotAddSeniorAdministratorRole), NotificationType.Warning);
                return RedirectToAction(nameof(Index));
            }
            var identityResult = await this.userManager.AddToRoleAsync(user, role);

            var success = identityResult.Succeeded;
            if (success)
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.UserAddedToRole, userEmail, role),
                    NotificationType.Success);
            }
            else
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }

            return RedirectToAction(nameof(Index));
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(string userEmail, string role)
        {
            if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(userEmail))
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToAction(nameof(Index));
            }

            var user = await this.userManager.FindByEmailAsync(userEmail);
            var currentLoggedUser = await this.userManager.GetUserAsync(User);

            if (user.Email == currentLoggedUser.Email)
            {
                this.ShowNotification(string.Format(NotificationMessages.UnableToRemoveSelf, role), NotificationType.Warning);
                return RedirectToAction(nameof(Index));
            }

            if (this.User.IsInRole(WebConstants.AdministratorRole) && role == WebConstants.SeniorAdministratorRole)
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.AdminCannotRemoveSeniorAdministratorRole), NotificationType.Warning);
                return RedirectToAction(nameof(Index));
            }

            var identityResult = await this.userManager.RemoveFromRoleAsync(user, role);

            var success = identityResult.Succeeded;
            if (success)
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.UserRemovedFromRole, userEmail, role),
                    NotificationType.Success);
            }
            else
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}