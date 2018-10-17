namespace CarDealer.Web.Areas.Ad.Models
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.Ad;

    public class AdDetailsViewModel : IMapWith<AdDetailsServiceModel>
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public bool IsReported { get; set; }

        public string PhoneNumber { get; set; }

        public AdDetailsVehicleModel Vehicle { get; set; }

        public string ReturnUrl { get; set; }
    }
}
