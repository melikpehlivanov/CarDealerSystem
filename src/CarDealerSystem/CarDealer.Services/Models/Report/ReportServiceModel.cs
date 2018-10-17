namespace CarDealer.Services.Models.Report
{
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class ReportServiceModel : IMapWith<Report>
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
