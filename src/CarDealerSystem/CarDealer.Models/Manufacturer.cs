namespace CarDealer.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Manufacturer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<Model> Models { get; set; }

        public IEnumerable<Vehicle> Vehicles { get; set; }
    }
}