using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;

namespace PetHaven.Controllers
{
    [Route("api/forums")]
    [ApiController]
    [Authorize]
    public class ForumsController : BaseController
    {
        private readonly IForumService _forumService;

        public ForumsController(IForumService forumService)
        {
            _forumService = forumService;
        }

        [HttpGet("threads")]
        public async Task<ActionResult<IEnumerable<ForumThread>>> GetAllThreads()
        {
            var threads = await _forumService.GetAllThreadsAsync();
            return Ok(threads);
        }

        [HttpGet("threads/{id}")]
        public async Task<ActionResult<ForumThread>> GetThreadById(int id)
        {
            var thread = await _forumService.GetThreadByIdAsync(id);
            if (thread == null)
            {
                return NotFound();
            }
            return Ok(thread);
        }

        [HttpPost("threads")]
        public async Task<ActionResult<ForumThread>> CreateThread([FromBody] CreateThreadDto threadDto)
        {
            threadDto.UserId = GetCurrentUserId();
            var thread = await _forumService.CreateThreadAsync(threadDto);
            return CreatedAtAction(nameof(GetThreadById), new { id = thread.Id }, thread);
        }

        [HttpPost("comments")]
        public async Task<ActionResult<ForumComment>> AddComment([FromBody] CreateCommentDto commentDto)
        {
            commentDto.UserId = GetCurrentUserId();
            var comment = await _forumService.AddCommentAsync(commentDto);
            return Ok(comment);
        }

        [HttpGet("threads/{threadId}/comments")]
        public async Task<ActionResult<IEnumerable<ForumComment>>> GetThreadComments(int threadId)
        {
            var comments = await _forumService.GetThreadCommentsAsync(threadId);
            return Ok(comments);
        }
    }
}