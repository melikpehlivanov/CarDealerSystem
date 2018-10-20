namespace CarDealer.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;
    using Common.Notifications;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;
    using Services.Models.Manufacturer;

    public class ManufacturerController : BaseController
    {
        private readonly IManufacturerService manufacturers;

        public ManufacturerController(IManufacturerService manufacturers)
        {
            this.manufacturers = manufacturers;
        }

        public async Task<IActionResult> Index()
        {
            var model = await this.manufacturers.AllAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await this.manufacturers.GetDetailedAsync(id);
            if (model == null)
            {
                this.ShowNotification(string.Format(NotificationMessages.ManufacturerDoesNotExist, id));
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var newManufacturerId = await this.manufacturers.CreateAsync(name);
            if (newManufacturerId == default(int))
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(string.Format(
                    NotificationMessages.ManufacturerCreatedSuccessfully, name),
                    NotificationType.Success);
            }

            return RedirectToAction(nameof(this.Details), new { id = newManufacturerId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await this.manufacturers.GetForUpdateAsync(id);
            if (model == null)
            {
                this.ShowNotification(string.Format(NotificationMessages.ManufacturerDoesNotExist, id));
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> Edit(ManufacturerUpdateServiceModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }
            var success = await this.manufacturers.UpdateAsync(model.Id, model.Name);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(string.Format(
                    NotificationMessages.ManufacturerUpdatedSuccessfully, model.Name),
                    NotificationType.Success);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.manufacturers.GetForUpdateAsync(id);
            if (model == null)
            {
                this.ShowNotification(string.Format(NotificationMessages.ManufacturerDoesNotExist, id));
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> Delete(ManufacturerUpdateServiceModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }
            var success = await this.manufacturers.DeleteAsync(model.Id);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(string.Format(
                    NotificationMessages.ManufacturerDeletedSuccessfully, model.Name),
                    NotificationType.Success);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
