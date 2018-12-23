namespace CarDealer.Services.Models.Vehicle
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class VehicleEditServiceModel : IMapWith<Vehicle>
    {
        public int Id { get; set; }
        
        public int ManufacturerId { get; set; }
        
        public string ModelName { get; set; }

        public string Description { get; set; }

        [Required]
        public int YearOfProduction { get; set; }

        public string Engine { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int EngineHorsePower { get; set; }

        [Required]
        public int FuelTypeId { get; set; }

        [Required]
        public int TransmissionTypeId { get; set; }

        public double TotalMileage { get; set; }

        public double FuelConsumption { get; set; }

        public decimal Price { get; set; }

        public List<Picture> Pictures { get; set; }

        public List<int> FeatureIds { get; set; }
    }
}
