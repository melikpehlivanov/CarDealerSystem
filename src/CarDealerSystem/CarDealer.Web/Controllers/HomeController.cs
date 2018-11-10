namespace CarDealer.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Services.Interfaces;

    public class HomeController : Controller
    {
        private readonly IManufacturerService manufacturers;

        public HomeController(IManufacturerService manufacturers)
        {
            this.manufacturers = manufacturers;
        }

        public async Task<IActionResult> Index()
        {
            var allManufacturers = await this.manufacturers.AllAsync();

            var model = new IndexViewModel
            {
                AllManufacturers = allManufacturers.Select(m => new SelectListItem(m.Name, m.Id.ToString())),
            };

            return View(model);
        }

        public IActionResult About() => View();

        public IActionResult Privacy() => View();

        public IActionResult Contact() => View();
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
