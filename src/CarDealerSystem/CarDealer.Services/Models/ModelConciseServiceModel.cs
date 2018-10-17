namespace CarDealer.Services.Models
{
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class ModelConciseServiceModel : IMapWith<Model>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string Name { get; set; }

        public int ManufacturerId { get; set; }

        [Display(Name = "Make")]
        public string ManufacturerName { get; set; }
    }
}
