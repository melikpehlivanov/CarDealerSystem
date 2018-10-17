using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(CarDealer.Web.Areas.Identity.IdentityHostingStartup))]
namespace CarDealer.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}