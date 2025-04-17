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
        public string AppointmentType { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Veterinarian { get; set; }

        public string? Reason { get; set; }

        public string Venue { get; set; }

        public string? Notes { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        public int PetId { get; set; }
        public virtual Pet? Pet { get; set; }

    }
}
