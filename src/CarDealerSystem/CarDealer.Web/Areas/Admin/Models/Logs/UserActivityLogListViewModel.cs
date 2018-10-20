namespace CarDealer.Web.Areas.Admin.Models.Logs
{
    using Infrastructure.Collections;
    using Services.Models.Logs;

    public class UserActivityLogListViewModel
    {
        public string SearchTerm { get; set; }

        public PaginatedList<UserActivityLogConciseServiceModel> Logs { get; set; }
    }
}
