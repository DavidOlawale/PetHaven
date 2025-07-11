﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;

namespace PetHaven.Controllers
{
    [ApiController]
    [Route("api/pet")]
    [Authorize]
    public class PetController : BaseController
    {
        private readonly IPetService _petService;

        public PetController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpGet]
        public IEnumerable<Pet> GetAllPet()
        {
            return _petService.GetAllPets();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPet(int id)
        {
            var pet = await _petService.GetPetByIdAsync(id);
            if (pet == null) return NotFound();

            return pet;
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<List<Pet>>> GetPetsByOwner(int ownerId)
        {
            return await _petService.GetPetsByOwnerIdAsync(ownerId);
        }


        [HttpPost]
        public async Task<IActionResult> AddPet([FromForm] AddPetDTO pet)
        {
            //if (petDto.Photo == null || petDto.Photo.Length == 0)
            //{
            //    return BadRequest("No file uploaded.");
            //}

            pet.OwnerID = GetCurrentUserId();
            var createdPet = await _petService.AddPetAsync(pet);
            return Ok(pet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePet(int id, Pet pet)
        {
            if (id != pet.Id)
            {
                return BadRequest();
            }

            await _petService.UpdatePetAsync(pet);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePet(int id)
        {
            await _petService.DeletePetAsync(id);
            return NoContent();
        }



        [HttpPost("Immunization")]
        public async Task<IActionResult> AddImmunization(Immunization immunization)
        {
            var createdImmunization = await _petService.AddPetImmunizationAsync(immunization);
            return Ok(createdImmunization);
        }

        [HttpGet("{petId}/Immunizations")]
        public IEnumerable<Immunization> GetPetImmunization(int petId)
        {
            return _petService.GetPetImmunizations(petId);
        }

        [HttpPut("Immunization")]
        public async Task<Immunization> UpdatePetImmunization(int petId, Immunization immunization)
        {
            return await _petService.UpdatePetImmunization(immunization.Id, immunization);
        }

        [HttpDelete("Immunization/{id}")]
        public async Task<IActionResult> DeletePetImmunization(int id)
        {
            await _petService.DeletePetImmunization(id);
            return NoContent();
        }



        [HttpPost("Medication")]
        public async Task<IActionResult> AddMedication(Medication medication)
        {
            var createdMedication = await _petService.AddPetMedicationAsync(medication);
            return Ok(createdMedication);
        }

        [HttpGet("{petId}/Medications")]
        public IEnumerable<Medication> GetPetMedication(int petId)
        {
            return _petService.GetPetMedications(petId);
        }

        [HttpPut("Medication")]
        public async Task<Medication> UpdatePetMedication(int petId, Medication medication)
        {
            return await _petService.UpdatePetMedication(medication.Id, medication);
        }

        [HttpDelete("Medication/{id}")]
        public async Task<IActionResult> DeletePetMedication(int id)
        {
            await _petService.DeletePetMedication(id);
            return NoContent();
        }




        [HttpPost("Appointment")]
        public async Task<IActionResult> AddAppointment(Appointment appointment)
        {
            var createdAppointment = await _petService.AddPetAppointmentAsync(appointment);
            return Ok(createdAppointment);
        }


        [HttpGet("{petId}/Appointments")]
        public IEnumerable<Appointment> GetPetAppointments(int petId)
        {
            return _petService.GetPetAppointments(petId);
        }

        [HttpPut("Appointment")]
        public async Task<Appointment> UpdatePetAppointment(int petId, Appointment appointment)
        {
            return await _petService.UpdatePetAppointment(appointment.Id, appointment);
        }

        [HttpDelete("Appointment/{id}")]
        public async Task<IActionResult> DeletePetAppointment(int id)
        {
            await _petService.DeletePetAppointment(id);
            return NoContent();
        }


    }
}