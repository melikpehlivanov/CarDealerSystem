namespace CarDealer.Services.Models.Vehicle
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using AutoMapper;
    using CarDealer.Models;
    using Common;
    using Common.AutoMapping.Interfaces;

    public class VehicleSearchServiceModel : IMapWith<Vehicle>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public int AdId { get; set; }

        [Display(Name = "Make")]
        public string ModelName { get; set; }
        
        public string ManufacturerName { get; set; }

        public int YearOfProduction { get; set; }
        
        public string Engine { get; set; }

        [Display(Name = "Miles")]
        public double TotalMileage { get; set; }
        
        public decimal Price { get; set; }

        public List<Picture> Pictures { get; set; }

        public string PrimaryPicturePath => GetPrimaryPicturePath(this.Pictures);

        private string GetPrimaryPicturePath(List<Picture> pictures)
        {
            if (!pictures.Any())
            {
                var defaultPicturePath = GlobalConstants.DefaultPicturePath;

                return defaultPicturePath;
            }
            var firstPic = pictures.First();

            return firstPic.Path;
        }

        public void ConfigureMapping(Profile mapper)
        {
            mapper.CreateMap<Vehicle, VehicleSearchServiceModel>()
                .ForMember(dest => dest.AdId,
                    opt => opt.MapFrom(src => src.Ads.Select(a => a.Id).FirstOrDefault()));
        }
    }
}
