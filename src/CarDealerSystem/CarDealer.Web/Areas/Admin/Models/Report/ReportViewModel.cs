namespace CarDealer.Web.Areas.Admin.Models.Report
{
    using Common.AutoMapping.Interfaces;
    using Services.Models.Report;

    public class ReportViewModel : IMapWith<ReportServiceModel>
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
