namespace CarDealer.Web.Areas.Ad.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using CarDealer.Models;
    using CarDealer.Models.BasicTypes;
    using Common;

    public class AdDetailsVehicleModel
    {
        public string FullModelName { get; set; }

        public string Description { get; set; }

        [Display(Name = "Fuel Type")]
        public string FuelTypeName { get; set; }

        public string TransmissionTypeName { get; set; }

        public string Engine { get; set; }

        public int EngineHorsePower { get; set; }

        [Display(Name = "Year")]
        public int YearOfProduction { get; set; }

        [Display(Name = "Mileage")]
        public double TotalMileage { get; set; }

        public double FuelConsumption { get; set; }

        public decimal Price { get; set; }

        public List<Picture> Pictures { get; set; }

        public List<Feature> Features { get; set; }

        public string PrimaryPicturePath => GetPrimaryPicturePath(this.Pictures);

        private string GetPrimaryPicturePath(IEnumerable<Picture> pictures)
        {
            if (!pictures.Any())
            {
                return $"{GlobalConstants.DefaultPicturePath}";
            }
            var firstPic = pictures.First();

            return firstPic?.Path;
        }
    }
}
