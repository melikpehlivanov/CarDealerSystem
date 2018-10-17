namespace CarDealer.Services.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;
    using Models.Ad;
    using Models.Report;
    using Models.User;
    using Models.Vehicle;

    public interface IAdService
    {
        Task<int> CreateAsync(VehicleCreateServiceModel model);

        Task<AdDetailsServiceModel> GetAsync(int id);

        string GetAdOwnerEmail(int id);

        IQueryable<UserAdsListingServiceModel> GetAllAdsByOwnerId(string userId);

        Task<AdEditServiceModel> GetForUpdateAsync(int id);

        Task<bool> DeleteAsync(int id);

        Task<bool> UpdateAsync(AdEditServiceModel serviceModel);

        Task<bool> UpdateReportedAdAsync(int id);

        IQueryable<ReportListingServiceModel> GetAllReportedAds();
    }
}
