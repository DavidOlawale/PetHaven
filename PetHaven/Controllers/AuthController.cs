using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PetHaven.Authentication;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;

namespace PetHaven.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly IUserService _UserService;

        public AuthController(IAuthService authService, IUserService userService, IJwtService jwtService)
        {
            _authService = authService;
            this._UserService = userService;
            this._jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO request)
        {
            var user = _UserService.GetUser(request.Email);

            if (user is not null && _authService.VerifyPasswordHash(user, request.Password, user!.PasswordHash))
            {
                var token = _jwtService.GenerateToken(user);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(SignUpDTO request)
        {
            var user = new User
            { 
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ZipCode = request.ZipCode,
                Role = UserRoles.AppUser
            };
            var registeredUser = await _authService.RegisterAsync(user, request.Password);
            return Created();
        }
    }

}
