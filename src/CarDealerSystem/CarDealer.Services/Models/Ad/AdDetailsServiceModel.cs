namespace CarDealer.Services.Models.Ad
{
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;
    using Vehicle;

    public class AdDetailsServiceModel : IMapWith<Ad>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string UserEmail { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsReported { get; set; }

        public string PhoneNumber { get; set; }

        public VehicleDetailsServiceModel Vehicle { get; set; }
    }
}
