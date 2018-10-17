namespace CarDealer.Services.Models.Manufacturer
{
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class ManufacturerUpdateServiceModel : IMapWith<Manufacturer>
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
