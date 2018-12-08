namespace CarDealer.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Notifications;
    using Infrastructure.Collections;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Logs;
    using Services.Interfaces;
    using Services.Models.Logs;

    [Authorize(Roles = WebConstants.SeniorAdministratorRole)]
    public class LogsController : BaseController
    {
        private readonly ILogService logService;

        public LogsController(ILogService logService)
        {
            this.logService = logService;
        }

        public IActionResult Index(string searchTerm, int page = 1)
        {
            page = Math.Max(1, page);
            var allLogs = this.logService.GetAll();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allLogs = allLogs.Where(u => u.UserEmail.ToLower().Contains(searchTerm.ToLower()));
            }
            
            var totalPages = (int)(Math.Ceiling(allLogs.Count() / (double)WebConstants.LogsListPageSize));
            page = Math.Min(page, Math.Max(1, totalPages));

            var logsToShow = allLogs
                .Skip((page - 1) * WebConstants.LogsListPageSize)
                .Take(WebConstants.LogsListPageSize)
                .ToList();

            var model = new UserActivityLogListViewModel
            {
                SearchTerm = searchTerm,
                Logs = new PaginatedList<UserActivityLogConciseServiceModel>(logsToShow, page, totalPages)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var logModel = await this.logService.GetAsync(id);
            if (logModel == null)
            {
                this.ShowNotification(string.Format(NotificationMessages.LogDoesNotExist, id));
                return RedirectToAction(nameof(Index));
            }

            return View(logModel);
        }
    }
}
