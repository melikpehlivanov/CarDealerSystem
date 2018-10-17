namespace CarDealer.Web.Areas.Vehicle.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Interfaces;

    [Area("Vehicle")]
    public class VehicleController : Controller
    {
        private readonly IVehicleService models;

        public VehicleController(IVehicleService models)
        {
            this.models = models;
        }

        [HttpGet]
        public async Task<JsonResult> GetModelsByManufacturerId(int manufacturerId)
        {
            var models = await this.models.GetByManufacturerIdAsync(manufacturerId);

            return this.Json(new SelectList(models));
        }
    }
}
