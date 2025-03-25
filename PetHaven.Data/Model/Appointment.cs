using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class Appointment: BaseEntity
    {

        [Required]
        [StringLength(50)]
        public string AppointmentType { get; set; } // e.g., "Vaccination", "Dental"

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Veterinarian { get; set; }

        public string? Reason { get; set; } // e.g., "Annual checkup"

        public string? Notes { get; set; } // Preparation instructions

        [Required]
        public bool IsCompleted { get; set; } = false;

        public int PetId { get; set; }
        public virtual Pet? Pet { get; set; }

    }
}
