using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class Reminder : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime ReminderDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        public TimeSpan? ReminderTime { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public int PetId { get; set; }
        public Pet Pet { get; set; }
    }
}
