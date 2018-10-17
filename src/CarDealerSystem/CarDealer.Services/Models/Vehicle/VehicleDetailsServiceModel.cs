namespace CarDealer.Services.Models.Vehicle
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class VehicleDetailsServiceModel : IMapWith<Vehicle>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string FullModelName { get; set; }

        [Display(Name = "Make")]
        public string ModelName { get; set; }

        public string ManufacturerName { get; set; }

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

        public List<VehicleFeature> Features { get; set; }
        
        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<Vehicle, VehicleDetailsServiceModel>()
                .ForMember(dest => dest.FullModelName,
                    opt => opt.MapFrom(src => $"{src.YearOfProduction} {src.Manufacturer.Name} {src.Model.Name}"))
                .ForMember(dest=> dest.Pictures,
                    opt=> opt.MapFrom(src=> src.Pictures))
                .ForMember(dest=> dest.Features,
                    opt=> opt.MapFrom(src=> src.Features));
        }
    }
}
