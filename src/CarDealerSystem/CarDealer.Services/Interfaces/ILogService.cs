namespace CarDealer.Services.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;
    using Models.Log;

    public interface ILogService
    {
        bool CreateUserActivityLog(UserActivityLogCreateModel model);

        IQueryable<UserActivityLogConciseServiceModel> GetAll();

        Task<UserActivityLogDetailsServiceModel> GetAsync(int id);
    }
}
