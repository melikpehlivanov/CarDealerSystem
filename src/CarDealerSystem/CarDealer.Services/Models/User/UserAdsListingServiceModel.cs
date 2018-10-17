namespace CarDealer.Services.Models.User
{
    using System;
    using Ad;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class UserAdsListingServiceModel : IMapWith<Ad>
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public DateTime CreationDate { get; set; }

        public AdVehicleListingServiceModel Vehicle { get; set; }
    }
}
