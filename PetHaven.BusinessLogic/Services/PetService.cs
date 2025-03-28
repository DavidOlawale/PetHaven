using PetHaven.API.Data;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IAzureBlobService _blobService;
        public PetService(IPetRepository petRepository, IAzureBlobService blobService)
        {
            _petRepository = petRepository;
            _blobService = blobService;
        }

        public async Task<Pet?> GetPetByIdAsync(int id)
        {
            return await _petRepository.GetPetByIdAsync(id);
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
            return await _petRepository.AddPetImmunizationAsync(immunization);
        }

        public async Task<Medication> AddPetMedicationAsync(Medication medication)
        {
            return await _petRepository.AddPetMedicationAsync(medication);
        }

        public async Task<Appointment> AddPetAppointmentAsync(Appointment appointment)
        {
            return await _petRepository.AddPetAppointmentAsync(appointment);
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
    }
}
