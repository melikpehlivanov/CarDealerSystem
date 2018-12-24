namespace CarDealer.Web.Areas.ContactUs.Controllers
{
    using System.Threading.Tasks;
    using Common.Notifications;
    using Infrastructure.Utilities.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Web.Controllers;

    [Area("ContactUs")]
    [Route("[area]/[action]/{id?}")]
    public class ContactController : Controller
    {
        private readonly IEmailSender emailSender;

        public ContactController(IEmailSender emailSender)
        {
            this.emailSender = emailSender;
        }

        public async Task<IActionResult> OnPostAsync(ContactUsFormViewModel model)
        {
            await this.emailSender.SendEmailAsync(model.Email, WebConstants.AppMainEmailAddress, model.Subject,
                model.Message);

            this.ShowNotification(NotificationMessages.EmailSentSuccessfully, NotificationType.Success);
            return RedirectToAction(nameof(HomeController.Contact), ControllersAndActionsConstants.HomeControllerName, new { area = "" });
        }

        private void ShowNotification(string message, NotificationType notificationType = NotificationType.Error)
        {
            this.TempData[NotificationConstants.NotificationMessageKey] = message;
            this.TempData[NotificationConstants.NotificationTypeKey] = notificationType.ToString();
        }
    }
}
