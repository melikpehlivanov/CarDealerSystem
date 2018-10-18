namespace CarDealer.Services.Implementations.Report
{
    using System.Threading.Tasks;
    using CarDealer.Models;
    using Data;
    using Interfaces;
    using Models.Report;

    public class ReportService : BaseService, IReportService
    {
        public ReportService(CarDealerDbContext db) 
            : base(db)
        {
        }

        public async Task<bool> CreateAsync(ReportServiceModel model)
        {
            var adToReport = await this.db.Ads
                .FindAsync(model.Id);

            if (adToReport == null)
            {
                return false;
            }

            adToReport.IsReported = true;

            var report = new Report
            {
                Description = model.Description,
                Ad = adToReport,
            };

            try
            {
                this.db.Update(adToReport);
                await this.db.Reports.AddAsync(report);
                await this.db.SaveChangesAsync();
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
