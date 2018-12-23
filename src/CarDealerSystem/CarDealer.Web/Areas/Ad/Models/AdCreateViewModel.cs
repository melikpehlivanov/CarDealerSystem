namespace CarDealer.Web.Areas.Ad.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Models;
    using CarDealer.Models.BasicTypes;
    using Common.AutoMapping.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.Vehicle;

    public class AdCreateViewModel : IMapWith<VehicleCreateServiceModel>
    {
        public string Description { get; set; }

        [Required]
        [Display(Name = "Make")]
        public int ManufacturerId { get; set; }

        public IEnumerable<SelectListItem> AllManufacturers { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string ModelName { get; set; }

        public IEnumerable<SelectListItem> AvailableYears { get; set; }
        
        [Required]
        [Display(Name = "Fuel type")]
        public int FuelTypeId { get; set; }
        
        public IEnumerable<SelectListItem> AllFuelTypes { get; set; }

        [Required]
        [Display(Name = "Transmission type")]
        public int TransmissionTypeId { get; set; }

        public IEnumerable<SelectListItem> AllTransmissionTypes { get; set; }

        public List<Feature> AllFeatures { get; set; }

        [Required]
        public string Engine { get; set; }
        
        public int EngineHorsePower { get; set; }

        [Required]
        [Display(Name = "Year")]
        public int YearOfProduction { get; set; }

        [Required]
        [Display(Name = "Mileage")]
        public double TotalMileage { get; set; }

        [Required]
        [Display(Name = "Fuel consumption per 100 km")]
        public int FuelConsumption { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string UserId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        
        public ICollection<Picture> Pictures { get; set; }
    }
}
