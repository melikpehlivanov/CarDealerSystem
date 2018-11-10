namespace CarDealer.Web.Infrastructure.Collections.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICache
    {
        Task<IEnumerable<SelectListItem>> GetAllManufacturersAsync();
        Task<IEnumerable<SelectListItem>> GetAllTransmissionTypesAsync();
        Task<IEnumerable<SelectListItem>> GetAllFuelTypesAsync();
    }
}
