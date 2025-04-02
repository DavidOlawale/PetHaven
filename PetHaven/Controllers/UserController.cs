using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([Required] int id, [FromBody] UpdateUserDTO updatedUser)
        {
            var result = await _userService.UpdateUserAsync(id, updatedUser);
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

    }
}