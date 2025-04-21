using Microsoft.AspNetCore.Mvc;
using PetHaven.Authentication;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;

namespace PetHaven.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseController
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
            var user = await _UserService.GetUserByEmailAsync(request.Email);

            if (user is not null && _authService.VerifyPasswordHash(user, request.Password, user!.PasswordHash))
            {
                var token = _jwtService.GenerateToken(user);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpDTO request)
        {
            var registeredUser = await _authService.RegisterAsync(request);
            var token = _jwtService.GenerateToken(registeredUser);
            return Ok(new { token });
        }

        [HttpGet("check-email")]
        public async Task<bool> CheckEmailExists([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return await _authService.CheckEmailExists(email);
        }
    }

}
