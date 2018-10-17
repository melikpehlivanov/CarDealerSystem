namespace CarDealer.Services.Models.Manufacturer
{
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class ManufacturerConciseListModel : IMapWith<Manufacturer>
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Models count")]
        public int ModelsCount { get; set; }
    }
}
