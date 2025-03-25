using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class Checkup: BaseEntity
    {

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckupDate { get; set; }

        [Required]
        [StringLength(100)]
        public string? Veterinarian { get; set; }

        [Required]
        [StringLength(500)]
        public string? Findings { get; set; }

        [StringLength(500)]
        public string? Recommendations { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Height { get; set; }
        public int PetId { get; set; }
        public virtual Pet? Pet { get; set; }

    }
}
