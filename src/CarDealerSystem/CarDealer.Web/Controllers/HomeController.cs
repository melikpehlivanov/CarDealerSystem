namespace CarDealer.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Infrastructure.Collections.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class HomeController : Controller
    {
        private readonly ICache cache;

        public HomeController(ICache cache)
        {
            this.cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var allManufacturers = await this.cache.GetAllManufacturersAsync();

            var model = new IndexViewModel
            {
                AllManufacturers = allManufacturers,
            };

            return View(model);
        }

        public IActionResult About() => View();

        public IActionResult Privacy() => View();

        public IActionResult Contact() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404)
                {
                    var viewName = $"Error{statusCode.ToString()}";
                    return View(viewName);
                }
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
