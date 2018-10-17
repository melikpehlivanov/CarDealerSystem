namespace CarDealer.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UserActivityLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; }

        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string HttpMethod { get; set; }

        [Required]
        public string ControllerName { get; set; }

        [Required]
        public string ActionName { get; set; }

        public string AreaName { get; set; }

        public string Url { get; set; }

        public string QueryString { get; set; }

        public string ActionArguments { get; set; }
    }
}
