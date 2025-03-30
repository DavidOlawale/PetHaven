using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.DTOs
{
    public class CreateProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public string Category { get; set; }
        public List<string>? Images { get; set; } = new List<string>();
        public int Stock { get; set; }
        public string? Brand { get; set; }
        public string? Weight { get; set; }
        public List<string> AnimalType { get; set; } = new List<string>();
    }
}
