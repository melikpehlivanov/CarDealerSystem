namespace CarDealer.Services.Models.User
{
    using System.Collections.Generic;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class UserListingServiceModel : IMapWith<User>
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public ICollection<string> CurrentRoles { get; set; }

        public ICollection<string> NonCurrentRoles { get; set; }
    }
}
