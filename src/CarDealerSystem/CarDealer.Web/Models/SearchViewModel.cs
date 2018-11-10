namespace CarDealer.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Infrastructure.Collections;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.Vehicle;

    public class SearchViewModel
    {
        private const int DefaultMaximumKilometers = 100000;
        private const int EngineHorsePowerMinValue = 10;

        public SearchViewModel(
            int manufacturerId,
            string modelName,
            decimal minPrice,
            decimal maxPrice,
            IEnumerable<SelectListItem> allManufacturers,
            IEnumerable<int> availableYears,
            int yearMin,
            IEnumerable<SelectListItem> allFuelTypes,
            IEnumerable<SelectListItem> allTransmissionTypes,
            int powerMin,
            int maximumKilometers,
            PaginatedList<VehicleSearchServiceModel> results,
            int totalCount)
        {
            this.ManufacturerId = manufacturerId;
            this.ModelName = modelName;
            this.MinPrice = minPrice;
            this.MaxPrice = maxPrice;
            this.AllManufacturers = allManufacturers;
            this.AvailableYears = availableYears.Select(y => new SelectListItem($"after {y.ToString()}", y.ToString()));
            this.AllFuelTypes = allFuelTypes;
            this.AllTransmissionTypes = allTransmissionTypes;
            this.MaximumKilometers = maximumKilometers != default(int) ? maximumKilometers : DefaultMaximumKilometers;
            this.EngineHorsePowerMin = powerMin != default(int) ? powerMin : EngineHorsePowerMinValue;
            this.YearOfManufactureMin = yearMin != default(int) ? yearMin : availableYears.Min();
            this.Results = results;
            this.TotalResultCount = totalCount;
        }

        public int Id { get; set; }

        [Display(Name = "Make")]
        public int ManufacturerId { get; set; }

        public IEnumerable<SelectListItem> AllManufacturers { get; set; }

        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Display(Name = "Minimum Price")]
        public decimal MinPrice { get; set; }

        [Display(Name = "Maximum Price")]
        public decimal MaxPrice { get; set; }

        [Display(Name = "Year")]
        public int YearOfManufactureMin { get; set; }

        public IEnumerable<SelectListItem> AvailableYears { get; set; }

        [Display(Name = "Power from")]
        public int EngineHorsePowerMin { get; set; }

        [Required]
        [Display(Name = "Fuel type")]
        public int FuelTypeId { get; set; }

        public IEnumerable<SelectListItem> AllFuelTypes { get; set; }

        [Required]
        [Display(Name = "Gearing type")]
        public int TransmissionTypeId { get; set; }

        public IEnumerable<SelectListItem> AllTransmissionTypes { get; set; }

        [Display(Name = "Maximum kilometers")]
        public int MaximumKilometers { get; set; }

        public PaginatedList<VehicleSearchServiceModel> Results { get; set; }

        public int TotalResultCount { get; set; }
    }
}
