namespace CarDealer.Web.Areas.Report.Models
{
    using Common.AutoMapping.Interfaces;
    using Services.Models;
    using Services.Models.Report;

    public class ReportViewModel : IMapWith<ReportServiceModel>
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
