namespace CarDealer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Ad
    {
        public int Id { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public Vehicle Vehicle { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        public bool IsReported { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Report> Reports { get; set; } = new List<Report>();
        
        public DateTime CreationDate { get; set; }
    }
}
