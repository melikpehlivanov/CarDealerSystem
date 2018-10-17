namespace CarDealer.Models.BasicTypes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Feature : BaseType
    {
        [NotMapped]
        public bool IsChecked { get; set; }

        public ICollection<VehicleFeature> Vehicles { get; set; } = new List<VehicleFeature>();
    }
}
