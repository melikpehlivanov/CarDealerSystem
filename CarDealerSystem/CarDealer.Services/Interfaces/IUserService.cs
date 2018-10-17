namespace CarDealer.Services.Interfaces
{
    using System.Linq;
    using Models;
    using Models.User;

    public interface IUserService
    {
        IQueryable<UserListingServiceModel> GetAll();
    }
}
