using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Interfaces
{
    public interface IPetService
    {
        Task<Pet?> GetPetByIdAsync(int id);
        Task<List<Pet>> GetPetsByOwnerIdAsync(int ownerId);
        Task<Pet> AddPetAsync(AddPetDTO pet);
        Task<Pet> UpdatePetAsync(Pet pet);
        Task DeletePetAsync(int id);

        Task<Immunization> AddPetImmunizationAsync(Immunization immunization);
        Task<Medication> AddPetMedicationAsync(Medication medication);
        Task<Appointment> AddPetAppointmentAsync(Appointment appointment);

        IEnumerable<Immunization> GetPetImmunizations(int petId);
        IEnumerable<Medication> GetPetMedications(int petId);
        IEnumerable<Appointment> GetPetAppointments(int petId);

        public Task<Immunization> UpdatePetImmunization(int immunizationId, Immunization immunization);
        public Task<Medication> UpdatePetMedication(int medicationId, Medication medication);
        public Task<Appointment> UpdatePetAppointment(int appointmentId, Appointment appointment);

        public Task DeletePetImmunization(int immunizationId);
        public Task DeletePetMedication(int medicationId);
        public Task DeletePetAppointment(int appointmentId);
    }
}
