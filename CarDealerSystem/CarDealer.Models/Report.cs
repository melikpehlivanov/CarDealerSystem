namespace CarDealer.Models
{
    public class Report
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int AdId { get; set; }

        public Ad Ad { get; set; }
    }
}
