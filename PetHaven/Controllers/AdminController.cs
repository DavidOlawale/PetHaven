using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;

namespace PetHaven.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPetService _petService;
        public AdminController(IUserService userService, IPetService petService)
        {
            _userService = userService;
            _petService = petService;
        }

        [HttpGet("users")]
        public IEnumerable<User> GetAllUsers()
        {
            return _userService.GetAllUsers();
        }

        [HttpGet("Appointments")]
        public IEnumerable<Appointment> GetAllAppointments()
        {
            return _petService.GetAllPetAppointments();
        }
    }
}