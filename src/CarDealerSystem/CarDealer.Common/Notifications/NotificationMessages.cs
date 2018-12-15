namespace CarDealer.Common.Notifications
{
    public class NotificationMessages
    {
        public const string InvalidOperation = "Oops something went wrong! Please try again";

        public const string UserAddedToRole = "User \"{0}\" successfully added to role \"{1}\"";
        public const string UserRemovedFromRole = "User \"{0}\" successfully removed from role \"{1}\"";
        public const string UnableToRemoveSelf = "You can not remove yourself from role {0}!";

        public const string AdminCannotAddSeniorAdministratorRole = "You don't have permission to add senior administrators. Only senior administrators have that permission.";
        public const string AdminCannotRemoveSeniorAdministratorRole = "You don't have permission to remove senior administrators.";
        
        public const string AdDoesNotExist = "The ad you are searching for does not exist!";
        public const string AdUpdatedSuccessfully = "Ad edited successfully";
        public const string AdDeletedSuccessfully = "Ad deleted successfully";

        public const string ManufacturerDoesNotExist = "Manufacturer with id \"{0}\" does not exist!";
        public const string ManufacturerCreatedSuccessfully = "Manufacturer \"{0}\" created successfully";
        public const string ManufacturerUpdatedSuccessfully = "Manufacturer \"{0}\" updated successfully";
        public const string ManufacturerDeletedSuccessfully = "Manufacturer \"{0}\" deleted successfully";

        public const string ModelCreatedSuccessfully = "Model \"{0}\" created successfully";
        public const string ModelUpdatedSuccessfully = "Model \"{0}\" updated successfully";
        public const string ModelDeletedSuccessfully = "Model \"{0}\" deleted successfully";

        public const string LogDoesNotExist = "Log \"{0}\" does not exist!";

        public const string NoReports = "Congratulations, today there aren't any reports to overview.";

        public const string ReportMarkedAsFalse =
            "Report was successfully marked as false. Thank you for reviewing this report and helping our team.";

        public const string ReportSubmittedSuccessfully =
            "Thank you for your report. One of our admins will review it as soon as possible.";

        public const string EmailSentSuccessfully = "Email was sent successfully.";
    }
}
