namespace CarDealer.Services.Models.Ad
{
    using System.Linq;
    using AutoMapper;
    using CarDealer.Models;
    using Common;
    using Common.AutoMapping.Interfaces;

    public class AdVehicleListingServiceModel : IMapWith<Vehicle>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string FullModelName { get; set; }

        public string PicturePath { get; set; }

        public decimal Price { get; set; }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<Vehicle, AdVehicleListingServiceModel>()
                .ForMember(dest => dest.FullModelName,
                    opt => opt.MapFrom(src => $"{src.YearOfProduction} {src.Manufacturer.Name} {src.Model.Name}"))
                .ForMember(dest => dest.PicturePath,
                    opt => opt.MapFrom(src => src.Pictures.Any() ? src.Pictures.First().Path : GlobalConstants.DefaultPicturePath));
        }
    }
}
