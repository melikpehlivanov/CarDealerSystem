namespace CarDealer.Web
{
    public class WebConstants
    {
        public const string AppMainEmailAddress = "mytestedcardealer@gmail.com";
        public const string SeniorAdministratorRole = "Senior Administrator";
        public const string SeniorAdministratorEmail = "seniorAdmin@cardealer.com";
        public const string SeniorAdministratorPassword = "admin123";
        public const string SeniorAndAdminRoles = "Senior Administrator,Administrator";

        public const string AdministratorRole = "Administrator";
        public const string AdministratorEmail = "admin@cardealer.com";
        public const string AdministratorPassword = "admin123";
        
        public const string ManufacturersPath = @"Resources\seedfiles\vehicles.json";
        public const string TransmissionTypesPath = @"Resources\seedfiles\transmission-types.json";
        public const string FuelTypesPath = @"Resources\seedfiles\fuel-types.json";
        public const string FeaturesPath = @"Resources\seedfiles\features.json";
        public const string UsersPath = @"Resources\seedfiles\users.json";

        public const int UsersListPageSize = 20;
        public const int SearchResultsPageSize = 24;
        public const int ReportsListPageSize = 20;
        public const int LogsListPageSize = 20;

        public const int StaticElementsCacheExpirationInDays = 10;

    }
}
