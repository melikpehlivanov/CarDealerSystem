namespace CarDealer.Web.Areas.Ad.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Common.AutoMapping.Interfaces;
    using Services.Models.Ad;

    public class AdEditViewModel : IMapWith<AdEditServiceModel>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        
        [Required]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public AdVehicleEdit Vehicle { get; set; }

        [Required]
        public IList<string> Urls { get; set; } = new List<string>();
    }
}
