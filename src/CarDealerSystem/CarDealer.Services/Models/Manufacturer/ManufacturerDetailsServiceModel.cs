namespace CarDealer.Services.Models.Manufacturer
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class ManufacturerDetailsServiceModel : IMapWith<Manufacturer>
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<ModelConciseServiceModel> Models { get; set; }
    }
}
