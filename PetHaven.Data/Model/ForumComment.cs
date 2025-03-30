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
        public int ForumThreadId { get; set; }
        public ForumThread ForumThread { get; set; }
    }
}
