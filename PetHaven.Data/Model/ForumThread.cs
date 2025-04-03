using System.ComponentModel.DataAnnotations;

namespace PetHaven.Data.Model
{
    public class ForumThread : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string Tags { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
