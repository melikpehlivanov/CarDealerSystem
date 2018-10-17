namespace CarDealer.Web.Infrastructure.Filters
{
    using System.Linq;
    using Areas.Ad.Controllers;
    using Areas.Vehicle.Controllers;
    using CarDealer.Models;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.EntityFrameworkCore;

    public class EnsureOwnership : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            var database = httpContext.RequestServices.GetService(typeof(CarDealerDbContext)) as CarDealerDbContext;
            var userManager = httpContext.RequestServices.GetService(typeof(UserManager<User>)) as UserManager<User>;
            var userId = userManager?.GetUserId(context.HttpContext.User);
            var adId = this.GetAdId(context);

            if (context.HttpContext.User.IsInRole(WebConstants.SeniorAdministratorRole) || context.HttpContext.User.IsInRole(WebConstants.AdministratorRole))
            {
                var dbAd = database?.Ads.AsNoTracking().FirstOrDefault(v => v.Id == adId);
                if (dbAd == null)
                {
                    context.Result = new LocalRedirectResult("/");
                }
            }
            else
            {
                var dbAd = database?.Ads.AsNoTracking().FirstOrDefault(v => v.Id == adId);
                if (dbAd == null || dbAd.UserId != userId)
                {
                    context.Result = new LocalRedirectResult("/");
                }
            }

            base.OnActionExecuting(context);
        }

        private int GetAdId(ActionExecutingContext context)
        {
            var actionArguments = context.ActionArguments;
            var controllerTypeName = context.Controller.GetType().Name;

            if (actionArguments.ContainsKey("vehicleId"))
            {
                return (int)actionArguments["vehicleId"];
            }

            if (actionArguments.ContainsKey("adId"))
            {
                return (int)actionArguments["adId"];
            }

            if (actionArguments.ContainsKey("id"))
            {
                var entityId = actionArguments["id"] as int?;

                switch (controllerTypeName)
                {
                    case nameof(VehicleController):
                        return entityId ?? default(int);
                    case nameof(AdController):
                        return entityId ?? default(int);
                    default:
                        return default(int);
                }
            }

            var referenceTypes = actionArguments.Where(aa => !aa.GetType().IsPrimitive && aa.GetType() != typeof(string)).Select(aa => aa.Value);
            foreach (var model in referenceTypes)
            {
                var vehicleIdProperty = controllerTypeName == nameof(AdController)
                    ? model.GetType()
                        .GetProperties()
                        .FirstOrDefault(pi => pi.Name.ToLower() == "vehicleId" || pi.Name.ToLower() == "adId")
                    : model.GetType()
                        .GetProperties()
                        .FirstOrDefault(pi => pi.Name.ToLower() == "id");

                if (vehicleIdProperty != null)
                {
                    if (int.TryParse(vehicleIdProperty.GetValue(model).ToString(), out int vehicleId))
                    {
                        return vehicleId;
                    }
                }
            }

            return default(int);
        }
    }
}