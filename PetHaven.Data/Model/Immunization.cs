using System.ComponentModel.DataAnnotations;

namespace PetHaven.Data.Model
{
    public class Immunization: BaseEntity
    {
        
        [Required]
        [MaxLength(100)]
        public string Vaccine { get; set; }

        [Required]
        public DateTime DateAdministered { get; set; }


        [Required]
        public DateTime NextDueDate { get; set; }

        public string? Notes { get; set; }

        public int PetId { get; set; }
        public virtual Pet? Pet { get; set; }

    }
}
