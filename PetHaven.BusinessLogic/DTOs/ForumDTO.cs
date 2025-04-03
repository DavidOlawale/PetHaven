using System;

    namespace PetHaven.BusinessLogic.DTOs
{
    public class CreateThreadDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public int UserId { get; set; }
    }
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public int ForumThreadId { get; set; }
        public int UserId { get; set; }
    }

    public class ForumThreadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Tags { get; set; }
        public string AuthorName { get; set; }
    }

    public class ForumCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorName { get; set; }
    }
}