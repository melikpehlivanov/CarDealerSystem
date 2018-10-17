using System.Collections.Generic;

namespace CarDealer.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class IndexViewModel
    {
        [Display(Name = "Make")]
        public IEnumerable<SelectListItem> AllManufacturers { get; set; }

        [Display(Name = "Make")]
        public int ManufacturerId { get; set; }

        [Display(Name = "Model")]
        public string ModelName { get; set; }
    }
}
