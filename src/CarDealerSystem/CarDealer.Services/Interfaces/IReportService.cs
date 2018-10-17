namespace CarDealer.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models;
    using Models.Report;

    public interface IReportService
    {
        Task<bool> CreateAsync(ReportServiceModel model);
    }
}
