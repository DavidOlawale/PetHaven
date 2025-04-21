using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class PetControllerTests
    {
        private readonly Mock<IPetService> _mockPetService;
        private readonly PetController _controller;
        private readonly int _testUserId = 5;

        public PetControllerTests()
        {
            _mockPetService = new Mock<IPetService>();
            _controller = new PetController(_mockPetService.Object);

            // Simulate authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Setup ControllerContext with HttpContext
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        #region Pet Tests

        [Fact]
        public void GetAllPet_ReturnsAllPets()
        {
            // Arrange
            var expectedPets = new List<Pet>
            {
                new Pet { Id = 1, Name = "Max", Type = PetType.Dog, Breed = "Golden Retriever", Weight = 30.5, OwnerId = 1 },
                new Pet { Id = 2, Name = "Bella", Type = PetType.Cat, Breed = "Siamese", Weight = 4.2, OwnerId = 2 }
            };

            _mockPetService.Setup(s => s.GetAllPets())
                .Returns(expectedPets);

            // Act
            var result = _controller.GetAllPet();

            // Assert
            var pets = Assert.IsAssignableFrom<IEnumerable<Pet>>(result);
            Assert.Equal(expectedPets.Count, ((List<Pet>)pets).Count);
            Assert.Equal(expectedPets, pets);
        }

        [Fact]
        public async Task GetPet_ReturnsOkWithPet_WhenPetExists()
        {
            // Arrange
            int petId = 1;
            var expectedPet = new Pet
            {
                Id = petId,
                Name = "Max",
                Type = PetType.Dog,
                Breed = "Golden Retriever",
                Weight = 30.5,
                OwnerId = 1
            };

            _mockPetService.Setup(s => s.GetPetByIdAsync(petId))
                .ReturnsAsync(expectedPet);

            // Act
            var result = await _controller.GetPet(petId);

            // Assert
            Assert.Equal(expectedPet, result.Value);
        }

        [Fact]
        public async Task GetPet_ReturnsNotFound_WhenPetDoesNotExist()
        {
            // Arrange
            int petId = 999;

            _mockPetService.Setup(s => s.GetPetByIdAsync(petId))
                .ReturnsAsync((Pet)null);

            // Act
            var result = await _controller.GetPet(petId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetPetsByOwner_ReturnsPetsForOwner()
        {
            // Arrange
            int ownerId = 1;
            var expectedPets = new List<Pet>
            {
                new Pet { Id = 1, Name = "Max", Type = PetType.Dog, Breed = "Golden Retriever", Weight = 30.5, OwnerId = ownerId },
                new Pet { Id = 3, Name = "Charlie", Type = PetType.Dog, Breed = "Beagle", Weight = 12.3, OwnerId = ownerId }
            };

            _mockPetService.Setup(s => s.GetPetsByOwnerIdAsync(ownerId))
                .ReturnsAsync(expectedPets);

            // Act
            var result = await _controller.GetPetsByOwner(ownerId);

            // Assert
            Assert.Equal(expectedPets, result.Value);
        }

        [Fact]
        public async Task AddPet_ReturnsOkWithPet_WhenSuccessful()
        {
            // Arrange
            var petDto = new AddPetDTO
            {
                Name = "Max",
                Type = PetType.Dog,
                Breed = "Golden Retriever",
                Weight = 30.5,
                DateOfBirth = DateTime.Now.AddYears(-2),
                Gender = "Male"
            };

            var createdPet = new Pet
            {
                Id = 1,
                Name = petDto.Name,
                Type = petDto.Type,
                Breed = petDto.Breed,
                Weight = petDto.Weight,
                DateOfBirth = petDto.DateOfBirth,
                Gender = petDto.Gender,
                OwnerId = _testUserId
            };

            _mockPetService.Setup(s => s.AddPetAsync(It.Is<AddPetDTO>(p =>
                p.Name == petDto.Name &&
                p.OwnerID == _testUserId)))
                .ReturnsAsync(createdPet);

            // Act
            var result = await _controller.AddPet(petDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Verify the DTO was updated with the correct owner ID
            Assert.Equal(_testUserId, petDto.OwnerID);

            // Verify service was called with the updated DTO
            _mockPetService.Verify(s => s.AddPetAsync(It.Is<AddPetDTO>(p =>
                p.Name == petDto.Name &&
                p.OwnerID == _testUserId)),
                Times.Once);
        }

        [Fact]
        public async Task UpdatePet_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int petId = 1;
            var pet = new Pet
            {
                Id = petId,
                Name = "Max Updated",
                Type = PetType.Dog,
                Breed = "Golden Retriever",
                Weight = 32.0,
                OwnerId = _testUserId
            };

            _mockPetService.Setup(s => s.UpdatePetAsync(pet))
                .ReturnsAsync(pet);

            // Act
            var result = await _controller.UpdatePet(petId, pet);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockPetService.Verify(s => s.UpdatePetAsync(pet), Times.Once);
        }

        [Fact]
        public async Task UpdatePet_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            int petId = 1;
            var pet = new Pet
            {
                Id = 2, // Different ID than the route parameter
                Name = "Max",
                Type = PetType.Dog,
                Breed = "Golden Retriever",
                Weight = 30.5,
                OwnerId = _testUserId
            };

            // Act
            var result = await _controller.UpdatePet(petId, pet);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mockPetService.Verify(s => s.UpdatePetAsync(It.IsAny<Pet>()), Times.Never);
        }

        [Fact]
        public async Task DeletePet_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int petId = 1;

            _mockPetService.Setup(s => s.DeletePetAsync(petId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePet(petId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockPetService.Verify(s => s.DeletePetAsync(petId), Times.Once);
        }

        #endregion

        #region Immunization Tests

        [Fact]
        public async Task AddImmunization_ReturnsOkWithImmunization_WhenSuccessful()
        {
            // Arrange
            var immunization = new Immunization
            {
                Vaccine = "Rabies",
                DateAdministered = DateTime.Today.AddDays(-30),
                NextDueDate = DateTime.Today.AddYears(1),
                PetId = 1
            };

            var createdImmunization = new Immunization
            {
                Id = 1,
                Vaccine = immunization.Vaccine,
                DateAdministered = immunization.DateAdministered,
                NextDueDate = immunization.NextDueDate,
                PetId = immunization.PetId
            };

            _mockPetService.Setup(s => s.AddPetImmunizationAsync(immunization))
                .ReturnsAsync(createdImmunization);

            // Act
            var result = await _controller.AddImmunization(immunization);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedImmunization = Assert.IsType<Immunization>(okResult.Value);
            Assert.Equal(createdImmunization, returnedImmunization);
        }

        [Fact]
        public void GetPetImmunization_ReturnsImmunizations()
        {
            // Arrange
            int petId = 1;
            var expectedImmunizations = new List<Immunization>
            {
                new Immunization
                {
                    Id = 1,
                    Vaccine = "Rabies",
                    DateAdministered = DateTime.Today.AddDays(-30),
                    NextDueDate = DateTime.Today.AddYears(1),
                    PetId = petId
                },
                new Immunization
                {
                    Id = 2,
                    Vaccine = "Distemper",
                    DateAdministered = DateTime.Today.AddDays(-60),
                    NextDueDate = DateTime.Today.AddYears(1),
                    PetId = petId
                }
            };

            _mockPetService.Setup(s => s.GetPetImmunizations(petId))
                .Returns(expectedImmunizations);

            // Act
            var result = _controller.GetPetImmunization(petId);

            // Assert
            var immunizations = Assert.IsAssignableFrom<IEnumerable<Immunization>>(result);
            Assert.Equal(expectedImmunizations.Count, ((List<Immunization>)immunizations).Count);
            Assert.Equal(expectedImmunizations, immunizations);
        }

        [Fact]
        public async Task UpdatePetImmunization_ReturnsUpdatedImmunization()
        {
            // Arrange
            int petId = 1;
            var immunization = new Immunization
            {
                Id = 1,
                Vaccine = "Rabies Updated",
                DateAdministered = DateTime.Today.AddDays(-15),
                NextDueDate = DateTime.Today.AddMonths(11),
                PetId = petId
            };

            _mockPetService.Setup(s => s.UpdatePetImmunization(immunization.Id, immunization))
                .ReturnsAsync(immunization);

            // Act
            var result = await _controller.UpdatePetImmunization(petId, immunization);

            // Assert
            Assert.Equal(immunization, result);
            _mockPetService.Verify(s => s.UpdatePetImmunization(immunization.Id, immunization), Times.Once);
        }

        [Fact]
        public async Task DeletePetImmunization_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int immunizationId = 1;

            _mockPetService.Setup(s => s.DeletePetImmunization(immunizationId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePetImmunization(immunizationId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockPetService.Verify(s => s.DeletePetImmunization(immunizationId), Times.Once);
        }

        #endregion

        #region Medication Tests

        [Fact]
        public async Task AddMedication_ReturnsOkWithMedication_WhenSuccessful()
        {
            // Arrange
            var medication = new Medication
            {
                Name = "Antibiotics",
                Dosage = "10mg",
                Frequency = "Twice daily",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(10),
                PetId = 1
            };

            var createdMedication = new Medication
            {
                Id = 1,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                PetId = medication.PetId
            };

            _mockPetService.Setup(s => s.AddPetMedicationAsync(medication))
                .ReturnsAsync(createdMedication);

            // Act
            var result = await _controller.AddMedication(medication);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMedication = Assert.IsType<Medication>(okResult.Value);
            Assert.Equal(createdMedication, returnedMedication);
        }

        [Fact]
        public void GetPetMedication_ReturnsMedications()
        {
            // Arrange
            int petId = 1;
            var expectedMedications = new List<Medication>
            {
                new Medication
                {
                    Id = 1,
                    Name = "Antibiotics",
                    Dosage = "10mg",
                    Frequency = "Twice daily",
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(10),
                    PetId = petId
                },
                new Medication
                {
                    Id = 2,
                    Name = "Pain Relief",
                    Dosage = "5mg",
                    Frequency = "Once daily",
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(5),
                    PetId = petId
                }
            };

            _mockPetService.Setup(s => s.GetPetMedications(petId))
                .Returns(expectedMedications);

            // Act
            var result = _controller.GetPetMedication(petId);

            // Assert
            var medications = Assert.IsAssignableFrom<IEnumerable<Medication>>(result);
            Assert.Equal(expectedMedications.Count, ((List<Medication>)medications).Count);
            Assert.Equal(expectedMedications, medications);
        }

        [Fact]
        public async Task UpdatePetMedication_ReturnsUpdatedMedication()
        {
            // Arrange
            int petId = 1;
            var medication = new Medication
            {
                Id = 1,
                Name = "Antibiotics Updated",
                Dosage = "15mg",
                Frequency = "Three times daily",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
                PetId = petId
            };

            _mockPetService.Setup(s => s.UpdatePetMedication(medication.Id, medication))
                .ReturnsAsync(medication);

            // Act
            var result = await _controller.UpdatePetMedication(petId, medication);

            // Assert
            Assert.Equal(medication, result);
            _mockPetService.Verify(s => s.UpdatePetMedication(medication.Id, medication), Times.Once);
        }

        [Fact]
        public async Task DeletePetMedication_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int medicationId = 1;

            _mockPetService.Setup(s => s.DeletePetMedication(medicationId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePetMedication(medicationId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockPetService.Verify(s => s.DeletePetMedication(medicationId), Times.Once);
        }

        #endregion

        #region Appointment Tests

        [Fact]
        public async Task AddAppointment_ReturnsOkWithAppointment_WhenSuccessful()
        {
            // Arrange
            var appointment = new Appointment
            {
                AppointmentType = "Checkup",
                ScheduledDate = DateTime.Today.AddDays(7),
                Veterinarian = "Dr. Smith",
                Reason = "Annual checkup",
                Venue = "Pet Clinic",
                PetId = 1
            };

            var createdAppointment = new Appointment
            {
                Id = 1,
                AppointmentType = appointment.AppointmentType,
                ScheduledDate = appointment.ScheduledDate,
                Veterinarian = appointment.Veterinarian,
                Reason = appointment.Reason,
                Venue = appointment.Venue,
                PetId = appointment.PetId,
                IsCompleted = false
            };

            _mockPetService.Setup(s => s.AddPetAppointmentAsync(appointment))
                .ReturnsAsync(createdAppointment);

            // Act
            var result = await _controller.AddAppointment(appointment);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAppointment = Assert.IsType<Appointment>(okResult.Value);
            Assert.Equal(createdAppointment, returnedAppointment);
        }

        [Fact]
        public void GetPetAppointments_ReturnsAppointments()
        {
            // Arrange
            int petId = 1;
            var expectedAppointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = 1,
                    AppointmentType = "Checkup",
                    ScheduledDate = DateTime.Today.AddDays(7),
                    Veterinarian = "Dr. Smith",
                    Reason = "Annual checkup",
                    Venue = "Pet Clinic",
                    PetId = petId,
                    IsCompleted = false
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentType = "Vaccination",
                    ScheduledDate = DateTime.Today.AddDays(14),
                    Veterinarian = "Dr. Johnson",
                    Reason = "Rabies shot",
                    Venue = "Pet Hospital",
                    PetId = petId,
                    IsCompleted = false
                }
            };

            _mockPetService.Setup(s => s.GetPetAppointments(petId))
                .Returns(expectedAppointments);

            // Act
            var result = _controller.GetPetAppointments(petId);

            // Assert
            var appointments = Assert.IsAssignableFrom<IEnumerable<Appointment>>(result);
            Assert.Equal(expectedAppointments.Count, ((List<Appointment>)appointments).Count);
            Assert.Equal(expectedAppointments, appointments);
        }

        [Fact]
        public async Task UpdatePetAppointment_ReturnsUpdatedAppointment()
        {
            // Arrange
            int petId = 1;
            var appointment = new Appointment
            {
                Id = 1,
                AppointmentType = "Checkup Updated",
                ScheduledDate = DateTime.Today.AddDays(10),
                Veterinarian = "Dr. Johnson",
                Reason = "Follow-up checkup",
                Venue = "Pet Hospital",
                PetId = petId,
                IsCompleted = true
            };

            _mockPetService.Setup(s => s.UpdatePetAppointment(appointment.Id, appointment))
                .ReturnsAsync(appointment);

            // Act
            var result = await _controller.UpdatePetAppointment(petId, appointment);

            // Assert
            Assert.Equal(appointment, result);
            _mockPetService.Verify(s => s.UpdatePetAppointment(appointment.Id, appointment), Times.Once);
        }

        [Fact]
        public async Task DeletePetAppointment_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int appointmentId = 1;

            _mockPetService.Setup(s => s.DeletePetAppointment(appointmentId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePetAppointment(appointmentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockPetService.Verify(s => s.DeletePetAppointment(appointmentId), Times.Once);
        }

        #endregion
    }
}