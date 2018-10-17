namespace CarDealer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using BasicTypes;
    using Enums;
    using static Common.ModelConstants;

    public class Vehicle
    {
        public int Id { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        public Manufacturer Manufacturer { get; set; }

        [Required]
        public int ModelId { get; set; }

        public Model Model { get; set; }

        public ConditionType Condition { get; set; }

        public string Description { get; set; }

        [Required]
        public int FuelTypeId { get; set; }

        public FuelType FuelType { get; set; }

        [Required]
        public int TransmissionTypeId { get; set; }

        public TransmissionType TransmissionType { get; set; }

        public string Engine { get; set; }

        [Range(EngineHorsePowerMinValue, int.MaxValue)]
        public int EngineHorsePower { get; set; }

        [Required]
        [Range(YearOfProductionMinValue, int.MaxValue)]
        public int YearOfProduction { get; set; }

        [Range(TotalMileageMinValue, double.MaxValue)]
        public double TotalMileage { get; set; }

        [Range(FuelConsumptionMinValue, double.MaxValue)]
        public double FuelConsumption { get; set; }

        [Required]
        [Range(typeof(decimal), PriceMinValue, PriceMaxValue)]
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }
        
        public ICollection<VehicleFeature> Features { get; set; } = new List<VehicleFeature>();

        public ICollection<Picture> Pictures { get; set; } = new List<Picture>();

        public ICollection<Ad> Ads { get; set; } = new List<Ad>();
    }
}
