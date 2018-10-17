namespace CarDealer.Web.Areas.Ad.Models
{
    using Infrastructure.Collections;
    using Services.Models.User;

    public class AdsListingViewModel
    {
        public string SearchTerm { get; set; }

        public PaginatedList<UserAdsListingServiceModel> Ads { get; set; }
    }
}
