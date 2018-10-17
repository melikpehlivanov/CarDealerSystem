namespace CarDealer.Services.Models.Report
{
    using System;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class ReportListingServiceModel : IMapWith<Ad>
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public int VehicleId { get; set; }

        public string FullModelName { get; set; }

        public string PicturePath { get; set; }
    }
}
