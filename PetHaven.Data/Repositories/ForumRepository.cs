using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories
{
    public class ForumRepository : IForumRepository
    {
        private readonly AppDbContext _context;

        public ForumRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ForumThread?> GetForumThreadByIdAsync(int threadId)
        {
            return await _context.ForumThreads.FindAsync(threadId);
        }

        public async Task<List<ForumThread>> GetAllForumThreadsAsync()
        {
            return await _context.ForumThreads
                .Include(t => t.User)
                .ToListAsync();
        }

        public async Task<ForumThread> AddForumThreadAsync(ForumThread thread)
        {
            _context.ForumThreads.Add(thread);
            await _context.SaveChangesAsync();
            return thread;
        }

        public async Task UpdateForumThreadAsync(ForumThread thread)
        {
            _context.ForumThreads.Update(thread);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteForumThreadAsync(int threadId)
        {
            var thread = await _context.ForumThreads.FindAsync(threadId);
            if (thread != null)
            {
                _context.ForumThreads.Remove(thread);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ForumComment> AddForumCommentAsync(ForumComment comment)
        {
            _context.ForumComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<ForumComment>> GetForumCommentsByThreadIdAsync(int threadId)
        {
            return await _context.ForumComments.Where(c => c.ForumThreadId == threadId)
                .Include(c => c.User)
                .ToListAsync();
        }
    }
}