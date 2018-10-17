namespace CarDealer.Web.Areas.Admin.Models.Report
{
    using Infrastructure.Collections;
    using Services.Models.Report;

    public class AdReportListingViewModel
    {
        public PaginatedList<ReportListingServiceModel> Results { get; set; }
    }
}
