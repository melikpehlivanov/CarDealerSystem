namespace CarDealer.Web.Areas.ContactUs.Controllers
{
    using System.Threading.Tasks;
    using Common.Notifications;
    using Identity.Pages.Services.Email;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using SendGrid;
    using SendGrid.Helpers.Mail;
    using Services.Interfaces;
    using Web.Controllers;

    [Area("ContactUs")]
    [Route("[area]/[action]/{id?}")]
    public class ContactController : Controller
    {
        private readonly IAdService ads;
        private readonly SendGridOptions options;
        
        public ContactController(IAdService ads, 
            IOptions<SendGridOptions> options)
        {
            this.ads = ads;
            this.options = options.Value;
        }

        public async Task<IActionResult> OnPostAsync(ContactUsFormViewModel model)
        {
            var adOwnerEmail = this.ads.GetAdOwnerEmail(model.ReceiverId);
            if (adOwnerEmail == null && !string.IsNullOrWhiteSpace(model.Receiver))
            {
                adOwnerEmail = model.Receiver;
            }

            var client = new SendGridClient(this.options.SendGridApiKey);
            var from = new EmailAddress(model.Email);
            var to = new EmailAddress(adOwnerEmail, adOwnerEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, model.Subject ,model.Message, model.Message);
            var response = await client.SendEmailAsync(msg);
            var body = await response.Body.ReadAsStringAsync();
            var statusCode = response.StatusCode;

            if (model.Receiver == WebConstants.AppMainEmailAddress)
            {
                this.ShowNotification(NotificationMessages.EmailSentSuccessfully, NotificationType.Success);
                return RedirectToAction(nameof(HomeController.Contact), "Home", new {area=""});
            }

            this.ShowNotification(NotificationMessages.EmailSentSuccessfully, NotificationType.Success);
            return RedirectToAction(nameof(Ad.Controllers.AdController.Details), "Ad", new { area="Ad", id = model.ReceiverId});
        }

        private void ShowNotification(string message, NotificationType notificationType = NotificationType.Error)
        {
            this.TempData[NotificationConstants.NotificationMessageKey] = message;
            this.TempData[NotificationConstants.NotificationTypeKey] = notificationType.ToString();
        }
    }
}
