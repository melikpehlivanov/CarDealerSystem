namespace CarDealer.Services.Models.Ad
{
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;
    using Vehicle;

    public class AdEditServiceModel : IMapWith<Ad>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public string PhoneNumber { get; set; }
        
        public VehicleEditServiceModel Vehicle { get; set; }
    }
}
