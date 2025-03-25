using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class Pet: BaseEntity
    {
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public PetType Type { get; set; }


        [MaxLength(50)]
        public string Breed { get; set; }

        [Required]
        public int Weight { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; }

        public string? Allergies { get; set; }

        public int? OwnerId { get; set; }

        public virtual User Owner { get; set; }

        [MaxLength(2048)]
        public string PhotoUrl { get; set; }

        public IEnumerable<Immunization> Immunizations { get; set; }
        public IEnumerable<Checkup> Checkups { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }

    }

    public enum PetType
    {
        Dog = 1,
        Cat,
        Rabbit,
        Bird
    }
}
