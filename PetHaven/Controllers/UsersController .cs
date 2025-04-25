using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using System.ComponentModel.DataAnnotations;

namespace PetHaven.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<User>>> GetUsers(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null,
            [FromQuery] string role = null,
            [FromQuery] string sortBy = "lastName",
            [FromQuery] string sortDirection = "asc")
        {
            var result = await _userService.GetPaginatedUsersAsync(page, pageSize, searchTerm, role, sortBy, sortDirection);
            return Ok(result);
        }
    }
}