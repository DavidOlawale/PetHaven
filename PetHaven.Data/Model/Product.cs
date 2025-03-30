using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class Product: BaseEntity
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public string Category { get; set; }
        public string ImageUrls { get; set; }
        public float Rating { get; set; } = 0;
        public int ReviewCount { get; set; } = 0;
        public int Stock { get; set; }
        public string? Brand { get; set; }
        public string? Weight { get; set; }
        public List<string> AnimalType { get; set; } = new List<string>();

    }
}
