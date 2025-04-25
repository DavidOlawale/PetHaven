using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace PetHaven.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(GetCurrentUserId());
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updatedUser)
        {
            var result = await _userService.UpdateUserAsync(GetCurrentUserId(), updatedUser);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("me")]
        public async Task<User?> GetCurrentUser()
        {
            var user = await _userService.GetUserByIdAsync(GetCurrentUserId());
            return user;
        }

        [SwaggerIgnore]
        [HttpPut("update-photo")]
        public async Task<ActionResult<User>> UpdateUserPhoto([FromForm] IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var userId = GetCurrentUserId();

            // Only allow image files
            if (!IsImageFile(photo.ContentType))
            {
                return BadRequest("File must be an image (JPEG, PNG, etc.)");
            }

            var updatedUser = await _userService.UpdateUserPhotoAsync(userId, photo);
            return Ok(updatedUser);
        }

        private bool IsImageFile(string contentType)
        {
            return contentType.StartsWith("image/");
        }

    }
}