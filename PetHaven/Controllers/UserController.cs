using Microsoft.AspNetCore.Mvc;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace PetHaven.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([Required] int id, [FromBody] User updatedUser)
        {
            var result = await _userService.UpdateUserAsync(updatedUser);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}