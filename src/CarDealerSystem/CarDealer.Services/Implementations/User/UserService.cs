namespace CarDealer.Services.Implementations.User
{
    using System.Linq;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Interfaces;
    using Models.User;

    public class UserService : BaseService, IUserService
    {
        private readonly IConfigurationProvider configuration;

        public UserService(CarDealerDbContext db, IMapper mapper)
            : base(db)
        {
            this.configuration = mapper.ConfigurationProvider;
        }

        public IQueryable<UserListingServiceModel> GetAll()
            => this.db
                 .Users
                 .OrderBy(u => u.Email)
                 .ProjectTo<UserListingServiceModel>(this.configuration);
    }
}
