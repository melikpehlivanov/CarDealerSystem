namespace CarDealer.Web.Areas.Ad.Controllers
{
    using Common.Notifications;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    [Authorize]
    [Area("Ad")]
    [Route("[area]/[action]/{id?}")]
    public abstract class BaseController : Controller
    {
        protected internal void ShowNotification(string message, NotificationType notificationType = NotificationType.Error)
        {
            this.TempData[NotificationConstants.NotificationMessageKey] = message;
            this.TempData[NotificationConstants.NotificationTypeKey] = notificationType.ToString();
        }

        public RedirectToActionResult RedirectToHome()
        {
            return RedirectToAction(nameof(Index), ControllersAndActionsConstants.HomeControllerName);
        }
    }
}
