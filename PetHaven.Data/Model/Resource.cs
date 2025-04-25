using System.ComponentModel.DataAnnotations;

namespace PetHaven.Data.Model
{
    public class Resource : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(2048)]
        public string ImageUrl { get; set; }

        [Required]
        public DateTime PublishedDate { get; set; }

        [MaxLength(50)]
        public string? Author { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        public int CreatorId { get; set; }
        public User? Creator { get; set; }
    }
}
