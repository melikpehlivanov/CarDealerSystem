namespace CarDealer.Web.Areas.Ad.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using CarDealer.Models;
    using CarDealer.Models.BasicTypes;
    using Common;
    using Common.AutoMapping.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services.Models.Vehicle;

    public class AdVehicleEdit : IMapWith<VehicleEditServiceModel>
    {
        [Required]
        public int Id { get; set; }

        public string Description { get; set; }
        
        [Display(Name = "Make")]
        public int ManufacturerId { get; set; }

        public IEnumerable<SelectListItem> AllManufacturers { get; set; }
        
        [Display(Name = "Model")]
        public string ModelName { get; set; }

        public IEnumerable<SelectListItem> AvailableYears { get; set; }

        [Display(Name = "Fuel type")]
        public int FuelTypeId { get; set; }

        public IEnumerable<SelectListItem> AllFuelTypes { get; set; }

        [Required]
        [Display(Name = "Transmission type")]
        public int TransmissionTypeId { get; set; }

        public IEnumerable<SelectListItem> AllTransmissionTypes { get; set; }

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
        public double FuelConsumption { get; set; }

        [Required]
        public decimal Price { get; set; }
        
        public List<Picture> Pictures { get; set; } = new List<Picture>();

        public List<Feature> AllFeatures { get; set; }

        public string PrimaryPicturePath => GetPrimaryPicturePath(this.Pictures);

        private string GetPrimaryPicturePath(IEnumerable<Picture> pictures)
        {
            if (!pictures.Any())
            {
                return GlobalConstants.DefaultPicturePath;
            }
            var firstPic = pictures.First();

            return firstPic.Path;
        }
    }
}
