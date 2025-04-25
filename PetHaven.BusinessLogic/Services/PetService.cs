
using Hangfire;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.BusinessLogic.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IBlobService _blobService;
        private readonly IEmailService _emailService;
        public PetService(IPetRepository petRepository, IBlobService blobService, IEmailService emailService)
        {
            _petRepository = petRepository;
            _blobService = blobService;
            _emailService = emailService;
        }

        public async Task<Pet?> GetPetByIdAsync(int id)
        {
            return await _petRepository.GetPetByIdAsync(id);
        }

        public IEnumerable<Pet> GetAllPets()
        {
            return _petRepository.GetAllPets();
        }

        public async Task<List<Pet>> GetPetsByOwnerIdAsync(int ownerId)
        {
            return await _petRepository.GetPetsByOwnerIdAsync(ownerId);
        }

        public async Task<Pet> AddPetAsync(AddPetDTO petDto)
        {
            var pet = new Pet
            {
                Name = petDto.Name,
                Type = petDto.Type,
                Breed = petDto.Breed,
                Gender = petDto.Gender,
                DateOfBirth = petDto.DateOfBirth,
                Weight = petDto.Weight,
                Allergies = petDto.Allergies,
                OwnerId = petDto.OwnerID
            };

            if (petDto.Photo != null && petDto.Photo.Length > 0)
            {
                var fileStream = petDto.Photo.OpenReadStream();
                var imageUrl = await _blobService.UploadImageAsync(fileStream);
                pet.PhotoUrl = imageUrl;
            }
            
            var createdPet = await _petRepository.AddPetAsync(pet);
            return createdPet;
        }

        public async Task<Pet> UpdatePetAsync(Pet pet)
        {
            await _petRepository.UpdatePetAsync(pet);
            return pet;
        }

        public async Task DeletePetAsync(int id)
        {
            await _petRepository.DeletePetAsync(id);
        }

        public async Task<Immunization> AddPetImmunizationAsync(Immunization immunization)
        {
            var dbImmunization =  await _petRepository.AddPetImmunizationAsync(immunization);
            var pet = await GetPetByIdAsync(dbImmunization.PetId);
            await _emailService.SendImmunizationNotificationAsync(pet.Owner, pet, dbImmunization);
            immunization.Pet = null; // Prevent objct circle error
            immunization.Id = dbImmunization.Id;
            return immunization;
        }

        public async Task<Medication> AddPetMedicationAsync(Medication medication)
        {
            var dbMedication = await _petRepository.AddPetMedicationAsync(medication);
            var pet = await GetPetByIdAsync(dbMedication.PetId);
            await _emailService.SendMedicationNotificationAsync(pet.Owner, pet, dbMedication);
            medication.Id = dbMedication.Id;
            return medication;
        }

        public async Task<Appointment> AddPetAppointmentAsync(Appointment appointment)
        {
            var dbAppointment = await _petRepository.AddPetAppointmentAsync(appointment);
            appointment.Id = dbAppointment.Id;

            try
            {
                var pet = await _petRepository.GetPetByIdAsync(appointment.PetId);

                // Calculate the reminder time (3 hours before the appointment)
                var reminderTime = appointment.ScheduledDate.AddHours(-3);

                // Schedule the reminder email using Hangfire

                BackgroundJob.Schedule<IPetService>(
                service => service.SendAppointmentReminderAsync(dbAppointment.Id),
                reminderTime - DateTime.UtcNow);

            }
            catch (Exception ex)
            {
                
            }

            return appointment;
        }

        public IEnumerable<Immunization> GetPetImmunizations(int petId)
        {
            return _petRepository.GetPetImmunizations(petId);
        }
        public IEnumerable<Medication> GetPetMedications(int petId)
        {
            return _petRepository.GetPetMedications(petId);
        }
        public IEnumerable<Appointment> GetPetAppointments(int petId)
        {
            return _petRepository.GetPetAppointments(petId);
        }
        public IEnumerable<Appointment> GetAllPetAppointments()
        {
            return _petRepository.GetAllPetAppointments();
        }

        public async Task<Immunization> UpdatePetImmunization(int immunizationId, Immunization immunization)
        {
            return await _petRepository.UpdatePetImmunization(immunizationId, immunization);
        }
        public async Task<Medication> UpdatePetMedication(int medicationId, Medication medication)
        {
            return await _petRepository.UpdatePetMedication(medicationId, medication);
        }
        public async Task<Appointment> UpdatePetAppointment(int appointmentId, Appointment appointment)
        {
            return await _petRepository.UpdatePetAppointment(appointmentId, appointment);
        }


        public async Task DeletePetImmunization(int immunizationId)
        {
            await _petRepository.DeletePetImmunization(immunizationId);
        }
        public async Task DeletePetMedication(int medicationId)
        {
            await _petRepository.DeletePetMedication(medicationId);
        }
        public async Task DeletePetAppointment(int appointmentId)
        {
            await _petRepository.DeletePetAppointment(appointmentId);
        }

        public async Task SendAppointmentReminderAsync(int appointmentId)
        {
            var appointment = await _petRepository.GetPetAppointment(appointmentId);
            await _emailService.SendAppointmentReminderAsync(appointment.Pet.Owner, appointment.Pet, appointment);
        }
    }
}
