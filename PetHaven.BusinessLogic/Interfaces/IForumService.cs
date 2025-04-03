using PetHaven.BusinessLogic.DTOs;
using PetHaven.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Interfaces
{
    public interface IForumService
    {
        Task<IEnumerable<ForumThread>> GetAllThreadsAsync();
        Task<ForumThread?> GetThreadByIdAsync(int id);
        Task<ForumThread> CreateThreadAsync(CreateThreadDto threadDto);
        Task<ForumComment> AddCommentAsync(CreateCommentDto commentDto);
        Task<IEnumerable<ForumComment>> GetThreadCommentsAsync(int threadId);
    }
}