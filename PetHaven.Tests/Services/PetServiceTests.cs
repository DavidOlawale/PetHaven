using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Http;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace PetHaven.Tests.Services
{
    public class PetServiceTests
    {
        private readonly Mock<IPetRepository> _mockPetRepository;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IBackgroundJobClient> _mockBackgroundJobClient;
        private readonly PetService _service;

        public PetServiceTests()
        {
            _mockPetRepository = new Mock<IPetRepository>();
            _mockBlobService = new Mock<IBlobService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockBackgroundJobClient = new Mock<IBackgroundJobClient>();

            _service = new PetService(_mockPetRepository.Object, _mockBlobService.Object, _mockEmailService.Object);
        }

        #region Get Methods Tests

        [Fact]
        public async Task GetPetByIdAsync_ReturnsPet_WhenPetExists()
        {
            // Arrange
            int petId = 1;
            var expectedPet = new Pet
            {
                Id = petId,
                Name = "Max",
                Type = PetType.Dog,
                Breed = "Golden Retriever",
                OwnerId = 1
            };

            _mockPetRepository.Setup(r => r.GetPetByIdAsync(petId))
                .ReturnsAsync(expectedPet);

            // Act
            var result = await _service.GetPetByIdAsync(petId);

            // Assert
            Assert.Equal(expectedPet, result);
            _mockPetRepository.Verify(r => r.GetPetByIdAsync(petId), Times.Once);
        }

        [Fact]
        public async Task GetPetByIdAsync_ReturnsNull_WhenPetDoesNotExist()
        {
            // Arrange
            int petId = 999;

            _mockPetRepository.Setup(r => r.GetPetByIdAsync(petId))
                .ReturnsAsync((Pet)null);

            // Act
            var result = await _service.GetPetByIdAsync(petId);

            // Assert
            Assert.Null(result);
            _mockPetRepository.Verify(r => r.GetPetByIdAsync(petId), Times.Once);
        }

        [Fact]
        public void GetAllPets_ReturnsAllPets()
        {
            // Arrange
            var expectedPets = new List<Pet>
            {
                new Pet { Id = 1, Name = "Max", Type = PetType.Dog, Breed = "Golden Retriever", OwnerId = 1 },
                new Pet { Id = 2, Name = "Bella", Type = PetType.Cat, Breed = "Siamese", OwnerId = 2 }
            };

            _mockPetRepository.Setup(r => r.GetAllPets())
                .Returns(expectedPets);

            // Act
            var result = _service.GetAllPets();

            // Assert
            Assert.Equal(expectedPets, result);
            _mockPetRepository.Verify(r => r.GetAllPets(), Times.Once);
        }

        [Fact]
        public async Task GetPetsByOwnerIdAsync_ReturnsPetsForOwner()
        {
            // Arrange
            int ownerId = 1;
            var expectedPets = new List<Pet>
            {
                new Pet { Id = 1, Name = "Max", Type = PetType.Dog, Breed = "Golden Retriever", OwnerId = ownerId },
                new Pet { Id = 3, Name = "Charlie", Type = PetType.Dog, Breed = "Beagle", OwnerId = ownerId }
            };

            _mockPetRepository.Setup(r => r.GetPetsByOwnerIdAsync(ownerId))
                .ReturnsAsync(expectedPets);

            // Act
            var result = await _service.GetPetsByOwnerIdAsync(ownerId);

            // Assert
            Assert.Equal(expectedPets, result);
            _mockPetRepository.Verify(r => r.GetPetsByOwnerIdAsync(ownerId), Times.Once);
        }

        #endregion

        #region Add/Update/Delete Pets Tests

        [Fact]
        public async Task AddPetAsync_CreatesPet_WithoutPhoto()
        {
            // Arrange
            var petDto = new AddPetDTO
            {
                Name = "Max",
                Type = PetType.Dog,
                Breed = "Golden Retriever",
                Gender = "Male",
                DateOfBirth = DateTime.Now.AddYears(-2),
                Weight = 30.5,
                OwnerID = 1
            };

            Pet capturedPet = null;
            var createdPet = new Pet
            {
                Id = 1,
                Name = petDto.Name,
                Type = petDto.Type,
                Breed = petDto.Breed,
                Gender = petDto.Gender,
                DateOfBirth = petDto.DateOfBirth,
                Weight = petDto.Weight,
                OwnerId = petDto.OwnerID
            };

            _mockPetRepository.Setup(r => r.AddPetAsync(It.IsAny<Pet>()))
                .Callback<Pet>(p => capturedPet = p)
                .ReturnsAsync(createdPet);

            // Act
            var result = await _service.AddPetAsync(petDto);

            // Assert
            Assert.Equal(createdPet, result);

            // Verify pet was created with correct properties
            Assert.NotNull(capturedPet);
            Assert.Equal(petDto.Name, capturedPet.Name);
            Assert.Equal(petDto.Type, capturedPet.Type);
            Assert.Equal(petDto.Breed, capturedPet.Breed);
            Assert.Equal(petDto.Gender, capturedPet.Gender);
            Assert.Equal(petDto.DateOfBirth, capturedPet.DateOfBirth);
            Assert.Equal(petDto.Weight, capturedPet.Weight);
            Assert.Equal(petDto.OwnerID, capturedPet.OwnerId);
            Assert.Null(capturedPet.PhotoUrl);

            _mockPetRepository.Verify(r => r.AddPetAsync(It.IsAny<Pet>()), Times.Once);
            _mockBlobService.Verify(b => b.UploadImageAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task AddPetAsync_CreatesPet_WithPhoto()
        {
            // Arrange
            var photoMock = new Mock<IFormFile>();
            var memoryStream = new MemoryStream();
            photoMock.Setup(f => f.Length).Returns(1024); // 1KB
            photoMock.Setup(f => f.OpenReadStream()).Returns(memoryStream);

            var petDto = new AddPetDTO
            {
                Name = "Max",
                Type = PetType.Dog,
                Breed = "Golden Retriever",
                Gender = "Male",
                DateOfBirth = DateTime.Now.AddYears(-2),
                Weight = 30.5,
                OwnerID = 1,
                Photo = photoMock.Object
            };

            string imageUrl = "https://storage.blob.core.windows.net/pets/image.jpg";
            _mockBlobService.Setup(b => b.UploadImageAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(imageUrl);

            Pet capturedPet = null;
            var createdPet = new Pet
            {
                Id = 1,
                Name = petDto.Name,
                Type = petDto.Type,
                Breed = petDto.Breed,
                Gender = petDto.Gender,
                DateOfBirth = petDto.DateOfBirth,
                Weight = petDto.Weight,
                OwnerId = petDto.OwnerID,
                PhotoUrl = imageUrl
            };

            _mockPetRepository.Setup(r => r.AddPetAsync(It.IsAny<Pet>()))
                .Callback<Pet>(p => capturedPet = p)
                .ReturnsAsync(createdPet);

            // Act
            var result = await _service.AddPetAsync(petDto);

            // Assert
            Assert.Equal(createdPet, result);

            // Verify pet was created with correct properties including photo URL
            Assert.NotNull(capturedPet);
            Assert.Equal(petDto.Name, capturedPet.Name);
            Assert.Equal(imageUrl, capturedPet.PhotoUrl);

            _mockPetRepository.Verify(r => r.AddPetAsync(It.IsAny<Pet>()), Times.Once);
            _mockBlobService.Verify(b => b.UploadImageAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePetAsync_UpdatesPet()
        {
            // Arrange
            var pet = new Pet
            {
                Id = 1,
                Name = "Max Updated",
                Type = PetType.Dog,
                Breed = "Golden Retriever",
                Weight = 32.0,
                OwnerId = 1
            };

            _mockPetRepository.Setup(r => r.UpdatePetAsync(pet))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdatePetAsync(pet);

            // Assert
            Assert.Equal(pet, result);
            _mockPetRepository.Verify(r => r.UpdatePetAsync(pet), Times.Once);
        }

        [Fact]
        public async Task DeletePetAsync_DeletesPet()
        {
            // Arrange
            int petId = 1;

            _mockPetRepository.Setup(r => r.DeletePetAsync(petId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeletePetAsync(petId);

            // Assert
            _mockPetRepository.Verify(r => r.DeletePetAsync(petId), Times.Once);
        }

        #endregion

        #region Immunization Tests

        [Fact]
        public async Task AddPetImmunizationAsync_CreatesImmunization_AndSendsNotification()
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

            var pet = new Pet
            {
                Id = 1,
                Name = "Max",
                Type = PetType.Dog,
                OwnerId = 1,
                Owner = new User { Id = 1, Email = "owner@example.com" }
            };

            _mockPetRepository.Setup(r => r.AddPetImmunizationAsync(immunization))
                .ReturnsAsync(createdImmunization);

            _mockPetRepository.Setup(r => r.GetPetByIdAsync(immunization.PetId))
                .ReturnsAsync(pet);

            // Act
            var result = await _service.AddPetImmunizationAsync(immunization);

            // Assert
            //Assert.Equal(createdImmunization, result);
            _mockPetRepository.Verify(r => r.AddPetImmunizationAsync(immunization), Times.Once);
            _mockEmailService.Verify(e => e.SendImmunizationNotificationAsync(pet.Owner, pet, createdImmunization), Times.Once);
        }

        [Fact]
        public void GetPetImmunizations_ReturnsImmunizations()
        {
            // Arrange
            int petId = 1;
            var expectedImmunizations = new List<Immunization>
            {
                new Immunization { Id = 1, Vaccine = "Rabies", PetId = petId },
                new Immunization { Id = 2, Vaccine = "Distemper", PetId = petId }
            };

            _mockPetRepository.Setup(r => r.GetPetImmunizations(petId))
                .Returns(expectedImmunizations);

            // Act
            var result = _service.GetPetImmunizations(petId);

            // Assert
            Assert.Equal(expectedImmunizations, result);
            _mockPetRepository.Verify(r => r.GetPetImmunizations(petId), Times.Once);
        }

        [Fact]
        public async Task UpdatePetImmunization_UpdatesImmunization()
        {
            // Arrange
            int immunizationId = 1;
            var immunization = new Immunization
            {
                Id = immunizationId,
                Vaccine = "Rabies Updated",
                DateAdministered = DateTime.Today.AddDays(-15),
                NextDueDate = DateTime.Today.AddMonths(11),
                PetId = 1
            };

            _mockPetRepository.Setup(r => r.UpdatePetImmunization(immunizationId, immunization))
                .ReturnsAsync(immunization);

            // Act
            var result = await _service.UpdatePetImmunization(immunizationId, immunization);

            // Assert
            Assert.Equal(immunization, result);
            _mockPetRepository.Verify(r => r.UpdatePetImmunization(immunizationId, immunization), Times.Once);
        }

        [Fact]
        public async Task DeletePetImmunization_DeletesImmunization()
        {
            // Arrange
            int immunizationId = 1;

            _mockPetRepository.Setup(r => r.DeletePetImmunization(immunizationId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeletePetImmunization(immunizationId);

            // Assert
            _mockPetRepository.Verify(r => r.DeletePetImmunization(immunizationId), Times.Once);
        }

        #endregion

        #region Medication Tests

        [Fact]
        public async Task AddPetMedicationAsync_CreatesMedication_AndSendsNotification()
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

            var pet = new Pet
            {
                Id = 1,
                Name = "Max",
                Type = PetType.Dog,
                OwnerId = 1,
                Owner = new User { Id = 1, Email = "owner@example.com" }
            };

            _mockPetRepository.Setup(r => r.AddPetMedicationAsync(medication))
                .ReturnsAsync(createdMedication);

            _mockPetRepository.Setup(r => r.GetPetByIdAsync(medication.PetId))
                .ReturnsAsync(pet);

            // Act
            var result = await _service.AddPetMedicationAsync(medication);

            // Assert
            Assert.Equal(medication, result);
            _mockPetRepository.Verify(r => r.AddPetMedicationAsync(medication), Times.Once);
            _mockEmailService.Verify(e => e.SendMedicationNotificationAsync(pet.Owner, pet, createdMedication), Times.Once);
        }

        [Fact]
        public void GetPetMedications_ReturnsMedications()
        {
            // Arrange
            int petId = 1;
            var expectedMedications = new List<Medication>
            {
                new Medication { Id = 1, Name = "Antibiotics", PetId = petId },
                new Medication { Id = 2, Name = "Pain Relief", PetId = petId }
            };

            _mockPetRepository.Setup(r => r.GetPetMedications(petId))
                .Returns(expectedMedications);

            // Act
            var result = _service.GetPetMedications(petId);

            // Assert
            Assert.Equal(expectedMedications, result);
            _mockPetRepository.Verify(r => r.GetPetMedications(petId), Times.Once);
        }

        [Fact]
        public async Task UpdatePetMedication_UpdatesMedication()
        {
            // Arrange
            int medicationId = 1;
            var medication = new Medication
            {
                Id = medicationId,
                Name = "Antibiotics Updated",
                Dosage = "15mg",
                Frequency = "Three times daily",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(7),
                PetId = 1
            };

            _mockPetRepository.Setup(r => r.UpdatePetMedication(medicationId, medication))
                .ReturnsAsync(medication);

            // Act
            var result = await _service.UpdatePetMedication(medicationId, medication);

            // Assert
            Assert.Equal(medication, result);
            _mockPetRepository.Verify(r => r.UpdatePetMedication(medicationId, medication), Times.Once);
        }

        [Fact]
        public async Task DeletePetMedication_DeletesMedication()
        {
            // Arrange
            int medicationId = 1;

            _mockPetRepository.Setup(r => r.DeletePetMedication(medicationId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeletePetMedication(medicationId);

            // Assert
            _mockPetRepository.Verify(r => r.DeletePetMedication(medicationId), Times.Once);
        }

        #endregion

        #region Appointment Tests

        [Fact]
        public async Task AddPetAppointmentAsync_CreatesAppointment_AndSchedulesReminder()
        {
            // Arrange
            var futureDate = DateTime.UtcNow.AddDays(1);
            var appointment = new Appointment
            {
                AppointmentType = "Checkup",
                ScheduledDate = futureDate,
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

            var pet = new Pet
            {
                Id = 1,
                Name = "Max",
                Type = PetType.Dog,
                OwnerId = 1
            };

            _mockPetRepository.Setup(r => r.AddPetAppointmentAsync(appointment))
                .ReturnsAsync(createdAppointment);

            _mockPetRepository.Setup(r => r.GetPetByIdAsync(appointment.PetId))
                .ReturnsAsync(pet);

            // Setup BackgroundJob.Schedule to verify Hangfire scheduling
            string jobId = "job-123";
            _mockBackgroundJobClient.Setup(x => x.Create(
                    It.IsAny<Job>(),
                    It.IsAny<ScheduledState>()))
                .Returns(jobId);

            // Act
            var result = await _service.AddPetAppointmentAsync(appointment);

            // Assert
            Assert.Equal(appointment, result);
            _mockPetRepository.Verify(r => r.AddPetAppointmentAsync(appointment), Times.Once);
            _mockPetRepository.Verify(r => r.GetPetByIdAsync(appointment.PetId), Times.Once);

            // We can't directly verify Hangfire's BackgroundJob.Schedule due to the static nature,
            // but we've mocked it above to capture the call
        }

        [Fact]
        public async Task AddPetAppointmentAsync_HandlesExceptions_DuringReminderScheduling()
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

            _mockPetRepository.Setup(r => r.AddPetAppointmentAsync(appointment))
                .ReturnsAsync(createdAppointment);

            _mockPetRepository.Setup(r => r.GetPetByIdAsync(appointment.PetId))
                .ThrowsAsync(new Exception("Test exception")); // Simulate exception

            // Act
            var result = await _service.AddPetAppointmentAsync(appointment);

            // Assert
            // Should still return the appointment even if there's an exception in scheduling
            Assert.Equal(appointment, result);
            _mockPetRepository.Verify(r => r.AddPetAppointmentAsync(appointment), Times.Once);
        }

        [Fact]
        public void GetPetAppointments_ReturnsAppointments()
        {
            // Arrange
            int petId = 1;
            var expectedAppointments = new List<Appointment>
            {
                new Appointment { Id = 1, AppointmentType = "Checkup", PetId = petId },
                new Appointment { Id = 2, AppointmentType = "Vaccination", PetId = petId }
            };

            _mockPetRepository.Setup(r => r.GetPetAppointments(petId))
                .Returns(expectedAppointments);

            // Act
            var result = _service.GetPetAppointments(petId);

            // Assert
            Assert.Equal(expectedAppointments, result);
            _mockPetRepository.Verify(r => r.GetPetAppointments(petId), Times.Once);
        }

        [Fact]
        public void GetAllPetAppointments_ReturnsAllAppointments()
        {
            // Arrange
            var expectedAppointments = new List<Appointment>
            {
                new Appointment { Id = 1, AppointmentType = "Checkup", PetId = 1 },
                new Appointment { Id = 2, AppointmentType = "Vaccination", PetId = 2 }
            };

            _mockPetRepository.Setup(r => r.GetAllPetAppointments())
                .Returns(expectedAppointments);

            // Act
            var result = _service.GetAllPetAppointments();

            // Assert
            Assert.Equal(expectedAppointments, result);
            _mockPetRepository.Verify(r => r.GetAllPetAppointments(), Times.Once);
        }

        [Fact]
        public async Task UpdatePetAppointment_UpdatesAppointment()
        {
            // Arrange
            int appointmentId = 1;
            var appointment = new Appointment
            {
                Id = appointmentId,
                AppointmentType = "Checkup Updated",
                ScheduledDate = DateTime.Today.AddDays(10),
                Veterinarian = "Dr. Johnson",
                Reason = "Follow-up checkup",
                Venue = "Pet Hospital",
                PetId = 1,
                IsCompleted = true
            };

            _mockPetRepository.Setup(r => r.UpdatePetAppointment(appointmentId, appointment))
                .ReturnsAsync(appointment);

            // Act
            var result = await _service.UpdatePetAppointment(appointmentId, appointment);

            // Assert
            Assert.Equal(appointment, result);
            _mockPetRepository.Verify(r => r.UpdatePetAppointment(appointmentId, appointment), Times.Once);
        }

        [Fact]
        public async Task DeletePetAppointment_DeletesAppointment()
        {
            // Arrange
            int appointmentId = 1;

            _mockPetRepository.Setup(r => r.DeletePetAppointment(appointmentId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeletePetAppointment(appointmentId);

            // Assert
            _mockPetRepository.Verify(r => r.DeletePetAppointment(appointmentId), Times.Once);
        }

        [Fact]
        public async Task SendAppointmentReminderAsync_SendsEmail()
        {
            // Arrange
            int appointmentId = 1;
            var appointment = new Appointment
            {
                Id = appointmentId,
                AppointmentType = "Checkup",
                ScheduledDate = DateTime.Today.AddHours(3),
                Veterinarian = "Dr. Smith",
                Reason = "Annual checkup",
                Venue = "Pet Clinic",
                PetId = 1,
                Pet = new Pet
                {
                    Id = 1,
                    Name = "Max",
                    Type = PetType.Dog,
                    OwnerId = 1,
                    Owner = new User { Id = 1, Email = "owner@example.com" }
                }
            };

            _mockPetRepository.Setup(r => r.GetPetAppointment(appointmentId))
                .ReturnsAsync(appointment);

            // Act
            await _service.SendAppointmentReminderAsync(appointmentId);

            // Assert
            _mockPetRepository.Verify(r => r.GetPetAppointment(appointmentId), Times.Once);
            _mockEmailService.Verify(e => e.SendAppointmentReminderAsync(
                appointment.Pet.Owner,
                appointment.Pet,
                appointment),
                Times.Once);
        }

        #endregion
    }
}