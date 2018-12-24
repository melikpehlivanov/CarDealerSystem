namespace CarDealer.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Common.Notifications;
    using Infrastructure.Collections;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Report;
    using Services.Interfaces;
    using Services.Models.Report;

    public class ReportController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAdService adService;
        private readonly IReportService reportService;

        public ReportController(
            IAdService adService,
            IReportService reportService,
            IMapper mapper)
        {
            this.adService = adService;
            this.reportService = reportService;
            this.mapper = mapper;
        }

        public IActionResult Index(int page = 1)
        {
            page = Math.Max(1, page);

            var allAds = this.adService.GetAllReportedAds();

            if (!allAds.Any())
            {
                this.ShowNotification(
                    string.Format(NotificationMessages.NoReports), NotificationType.Success);
            }

            var totalPages = (int)(Math.Ceiling(allAds.Count() / (double)WebConstants.ReportsListPageSize));
            page = Math.Min(page, Math.Max(1, totalPages));

            var adsToShow = allAds
                .Skip((page - 1) * WebConstants.ReportsListPageSize)
                .Take(WebConstants.ReportsListPageSize)
                .ToList();

            var model = new AdReportListingViewModel
            {
                Results = new PaginatedList<ReportListingServiceModel>(adsToShow, page, totalPages),
            };

            return View(model);
        }

        [AllowAnonymous]
        [Route("[controller]/[action]/{id?}")]
        public IActionResult Create(int id) => View();

        [AllowAnonymous]
        [Route("[controller]/[action]/{id?}")]
        [HttpPost]
        public async Task<IActionResult> Submit(ReportViewModel model)
        {
            var serviceModel = this.mapper.Map<ReportServiceModel>(model);

            var success = await this.reportService.CreateAsync(serviceModel);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToAdDetails(model.Id);
            }

            this.ShowNotification(NotificationMessages.ReportSubmittedSuccessfully, NotificationType.Success);
            return RedirectToAdDetails(model.Id);
        }

        [Log]
        [HttpPost]
        public async Task<IActionResult> Update(int id)
        {
            var success = await this.adService.UpdateReportedAdAsync(id);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
            }
            else
            {
                this.ShowNotification(NotificationMessages.ReportMarkedAsFalse, NotificationType.Success);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
