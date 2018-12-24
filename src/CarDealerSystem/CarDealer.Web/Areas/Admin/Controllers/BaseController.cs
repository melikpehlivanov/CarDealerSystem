namespace CarDealer.Web.Areas.Admin.Controllers
{
    using Ad.Controllers;
    using Common.Notifications;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("Admin")]
    [Authorize(Roles = WebConstants.SeniorAndAdminRoles)]
    public abstract class BaseController : Controller
    {
        protected internal void ShowNotification(string message, NotificationType notificationType = NotificationType.Error)
        {
            this.TempData[NotificationConstants.NotificationMessageKey] = message;
            this.TempData[NotificationConstants.NotificationTypeKey] = notificationType.ToString();
        }

        public RedirectToActionResult RedirectToAdDetails(int id)
            => RedirectToAction(nameof(AdController.Details),
                ControllersAndActionsConstants.AdControllerConstants.ControllerAndAreaName,
                new { area = ControllersAndActionsConstants.AdControllerConstants.ControllerAndAreaName, id });

        public RedirectToActionResult RedirectToManufacturerDetails(int id)
            => RedirectToAction(nameof(ManufacturerController.Details),
                ControllersAndActionsConstants.ManufacturerControllerConstants.ControllerAndAreaName, new { id });

        public RedirectToActionResult RedirectToManufacturerIndex()
            => RedirectToAction(nameof(ManufacturerController.Index), ControllersAndActionsConstants.ManufacturerControllerConstants.ControllerAndAreaName);

    }
}
