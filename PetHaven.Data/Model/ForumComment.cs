using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class ForumComment : BaseEntity
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        
        //public int? ParentCommentId { get; set; }
        //public ForumComment ParentComment { get; set; }


        [Required]
        public int ForumThreadId { get; set; }

        public ForumThread ForumThread { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
