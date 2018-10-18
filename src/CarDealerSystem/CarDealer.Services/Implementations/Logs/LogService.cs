namespace CarDealer.Services.Implementations.Logs
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using CarDealer.Models;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;
    using Models.Logs;

    public class LogService : BaseService, ILogService
    {
        private readonly IMapper mapper;

        public LogService(CarDealerDbContext db, IMapper mapper) 
            : base(db)
        {
            this.mapper = mapper;
        }

        public bool CreateUserActivityLog(UserActivityLogCreateModel model)
        {
            var log = this.mapper.Map<UserActivityLog>(model);

            try
            {
                this.ValidateEntityState(log);
                this.db.Logs.Add(log);
                this.db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IQueryable<UserActivityLogConciseServiceModel> GetAll()
        {
            var logs = this.db
                .Logs
                .OrderByDescending(l => l.DateTime)
                .ProjectTo<UserActivityLogConciseServiceModel>(this.mapper.ConfigurationProvider);

            return logs;
        }

        public Task<UserActivityLogDetailsServiceModel> GetAsync(int id)
            => this.db
                .Logs
                .ProjectTo<UserActivityLogDetailsServiceModel>(this.mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(l => l.Id == id);
    }
}
