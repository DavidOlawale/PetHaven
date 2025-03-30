using PetHaven.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories.Interfaces
{
    public interface IForumRepository
    {
        Task<ForumThread?> GetForumThreadByIdAsync(int threadId);
        Task<List<ForumThread>> GetAllForumThreadsAsync();
        Task<ForumThread> AddForumThreadAsync(ForumThread thread);
        Task UpdateForumThreadAsync(ForumThread thread);
        Task DeleteForumThreadAsync(int threadId);
        Task<ForumComment> AddForumCommentAsync(ForumComment comment);
        Task<List<ForumComment>> GetForumCommentsByThreadIdAsync(int threadId);
    }
}