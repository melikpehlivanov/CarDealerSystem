namespace CarDealer.Models.BasicTypes
{
    using System.ComponentModel.DataAnnotations;

    public abstract class BaseType
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public override string ToString() => this.Name;
    }
}
