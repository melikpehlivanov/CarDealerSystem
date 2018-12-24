namespace CarDealer.Web
{
    using Areas.Ad.Controllers;
    using Areas.Admin.Controllers;

    public class ControllersAndActionsConstants
    {
        public const string HomeControllerName = "Home";

        public class AdControllerConstants
        {
            public static readonly string ControllerAndAreaName = 
                nameof(AdController).Replace("Controller", string.Empty).Trim();
        }

        public class ManufacturerControllerConstants
        {
            public static readonly string ControllerAndAreaName =
                nameof(ManufacturerController).Replace("Controller", string.Empty).Trim();
        }
    }
}
