using Moq;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class JwtServiceTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IPetService> _mockPetService;
        private readonly AdminController _controller;

        public JwtServiceTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockPetService = new Mock<IPetService>();
            _controller = new AdminController(_mockUserService.Object, _mockPetService.Object);
        }

        [Fact]
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" },
                new User { Id = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com" }
            };

            _mockUserService.Setup(s => s.GetAllUsers())
                .Returns(expectedUsers);

            // Act
            var result = _controller.GetAllUsers();

            // Assert
            var userList = Assert.IsAssignableFrom<IEnumerable<User>>(result);
            Assert.Equal(expectedUsers.Count, ((List<User>)userList).Count);
            Assert.Equal(expectedUsers, userList);
        }

        [Fact]
        public void GetAllUsers_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var emptyList = new List<User>();

            _mockUserService.Setup(s => s.GetAllUsers())
                .Returns(emptyList);

            // Act
            var result = _controller.GetAllUsers();

            // Assert
            var userList = Assert.IsAssignableFrom<IEnumerable<User>>(result);
            Assert.Empty(userList);
        }

        [Fact]
        public void GetAllAppointments_ReturnsAllAppointments()
        {
            // Arrange
            var expectedAppointments = new List<Appointment>
            {
                new Appointment { Id = 1, PetId = 1, ScheduledDate = DateTime.Now.AddDays(1), AppointmentType = "Checkup" },
                new Appointment { Id = 2, PetId = 2, ScheduledDate = DateTime.Now.AddDays(2), AppointmentType = "Vaccination" },
                new Appointment { Id = 3, PetId = 1, ScheduledDate = DateTime.Now.AddDays(3), AppointmentType = "Surgery" }
            };

            _mockPetService.Setup(s => s.GetAllPetAppointments())
                .Returns(expectedAppointments);

            // Act
            var result = _controller.GetAllAppointments();

            // Assert
            var appointmentList = Assert.IsAssignableFrom<IEnumerable<Appointment>>(result);
            Assert.Equal(expectedAppointments.Count, ((List<Appointment>)appointmentList).Count);
            Assert.Equal(expectedAppointments, appointmentList);
        }

        [Fact]
        public void GetAllAppointments_ReturnsEmptyList_WhenNoAppointmentsExist()
        {
            // Arrange
            var emptyList = new List<Appointment>();

            _mockPetService.Setup(s => s.GetAllPetAppointments())
                .Returns(emptyList);

            // Act
            var result = _controller.GetAllAppointments();

            // Assert
            var appointmentList = Assert.IsAssignableFrom<IEnumerable<Appointment>>(result);
            Assert.Empty(appointmentList);
        }
    }
}