using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class ForumService : IForumService
    {
        private readonly IForumRepository _forumRepository;

        public ForumService(IForumRepository forumRepository)
        {
            _forumRepository = forumRepository;
        }

        public async Task<IEnumerable<ForumThread>> GetAllThreadsAsync()
        {
            return await _forumRepository.GetAllForumThreadsAsync();
        }

        public async Task<ForumThread?> GetThreadByIdAsync(int id)
        {
            return await _forumRepository.GetForumThreadByIdAsync(id);
        }

        public async Task<ForumThread> CreateThreadAsync(CreateThreadDto threadDto)
        {
            var thread = new ForumThread
            {
                Title = threadDto.Title,
                Content = threadDto.Content,
                Tags = threadDto.Tags,
                UserId = threadDto.UserId,
                CreatedDate = DateTime.UtcNow
            };

            return await _forumRepository.AddForumThreadAsync(thread);
        }

        public async Task<ForumComment> AddCommentAsync(CreateCommentDto commentDto)
        {
            var comment = new ForumComment
            {
                Content = commentDto.Content,
                ForumThreadId = commentDto.ForumThreadId,
                UserId = commentDto.UserId,
                CreatedDate = DateTime.UtcNow
            };

            return await _forumRepository.AddForumCommentAsync(comment);
        }

        public async Task<IEnumerable<ForumComment>> GetThreadCommentsAsync(int threadId)
        {
            return await _forumRepository.GetForumCommentsByThreadIdAsync(threadId);
        }
    }
}