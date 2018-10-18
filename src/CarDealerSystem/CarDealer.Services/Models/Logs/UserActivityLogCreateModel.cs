namespace CarDealer.Services.Models.Logs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Models;
    using Common.AutoMapping.Interfaces;

    public class UserActivityLogCreateModel : IMapWith<UserActivityLog>
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string HttpMethod { get; set; }

        [Required]
        public string ControllerName { get; set; }

        [Required]
        public string ActionName { get; set; }

        public string AreaName { get; set; }

        [Url]
        public string Url { get; set; }

        public string QueryString { get; set; }

        public string ActionArguments { get; set; }
    }
}
