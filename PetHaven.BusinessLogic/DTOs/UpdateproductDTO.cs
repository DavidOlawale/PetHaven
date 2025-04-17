using Microsoft.AspNetCore.Http;
using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetHaven.BusinessLogic.DTOs
{
    public class UpdateProductDto
    {
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0.01, 9999.99)]
        public decimal DiscountedPrice { get; set; }

        [Range(0, 9999.99)]
        public decimal OriginalPrice { get; set; }

        public string Category { get; set; }

        public string ExistingImageUrls { get; set; }

        public int Stock { get; set; }

        public string? Brand { get; set; }

        public string? Weight { get; set; }

        public IEnumerable<string>? NewImages { get; set; }

    }
}
