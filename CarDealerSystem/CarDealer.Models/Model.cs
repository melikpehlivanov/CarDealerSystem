namespace CarDealer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Model
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ManufacturerId { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public IEnumerable<Vehicle> Vehicles { get; set; } 
    }
}