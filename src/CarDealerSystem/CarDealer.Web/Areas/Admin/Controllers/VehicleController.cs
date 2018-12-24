namespace CarDealer.Web.Areas.Admin.Controllers
{
    using System.Threading.Tasks;
    using Common.Notifications;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;
    using Services.Models;

    public class VehicleController : BaseController
    {
        private readonly IVehicleService vehicleModels;

        public VehicleController(IVehicleService vehicleModels)
        {
            this.vehicleModels = vehicleModels;
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> Create(string name, int manufacturerId)
        {
            var success = await this.vehicleModels.CreateAsync(name, manufacturerId);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.ModelCreatedSuccessfully, name),
                    NotificationType.Success);
            }
            
            return RedirectToManufacturerDetails(manufacturerId);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await this.vehicleModels.GetAsync(id);
            if (model == null)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToManufacturerIndex();
            }

            return View(model);
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> Edit(ModelConciseServiceModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }
            bool success = await this.vehicleModels.UpdateAsync(model.Id, model.Name);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(string.Format(NotificationMessages.ModelUpdatedSuccessfully, model.Name), NotificationType.Success);
            }

            return RedirectToManufacturerDetails(model.ManufacturerId);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.vehicleModels.GetAsync(id);
            if (model == null)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToManufacturerIndex();
            }

            return View(model);
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> Delete(ModelConciseServiceModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }
            var success = await this.vehicleModels.DeleteAsync(model.Id);
            if (success)
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.ModelDeletedSuccessfully, model.Name),
                    NotificationType.Success);
            }
            else
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }

            return RedirectToManufacturerDetails(model.ManufacturerId);
        }
    }
}
