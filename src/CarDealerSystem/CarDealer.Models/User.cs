namespace CarDealer.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser
    {
        public ICollection<Ad> Ads { get; set; } = new List<Ad>();
    }
}
