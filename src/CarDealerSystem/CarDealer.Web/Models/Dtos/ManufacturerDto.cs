namespace CarDealer.Web.Models.Dtos
{
    using System.Collections.Generic;

    public class ManufacturerDto
    {
        public string Name { get; set; }

        public IEnumerable<string> Models { get; set; }
    }
}
