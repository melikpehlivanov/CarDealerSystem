namespace CarDealer.Services.Models.Vehicle
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class VehicleCreateServiceModel : IMapWith<Vehicle>, IHaveCustomMapping
    {
        [Required]
        public int ManufacturerId { get; set; }

        [Required]
        public string ModelName { get; set; }

        public string Description { get; set; }

        [Required]
        public int YearOfProduction { get; set; }

        public string Engine { get; set; }

        [Required]
        public int EngineHorsePower { get; set; }

        [Required]
        public int FuelTypeId { get; set; }

        [Required]
        public int TransmissionTypeId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        
        public double TotalMileage { get; set; }
        
        public double FuelConsumption { get; set; }

        public decimal Price { get; set; }
        
        public List<int> PictureIds { get; set; }

        public List<int> FeatureIds { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime CreationDate { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<VehicleCreateServiceModel, Ad>()
                .ForMember(dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId));
        }
    }
}
