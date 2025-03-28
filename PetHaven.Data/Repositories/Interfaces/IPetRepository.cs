using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories.Interfaces
{
    public interface IPetRepository
    {
        Task<Pet?> GetPetByIdAsync(int id);
        Task<List<Pet>> GetPetsByOwnerIdAsync(int ownerId);
        Task<Pet> AddPetAsync(Pet pet);
        Task UpdatePetAsync(Pet pet);
        Task DeletePetAsync(int id);
        Task<Immunization> AddPetImmunizationAsync(Immunization immunization);
        Task<Medication> AddPetMedicationAsync(Medication medication);
        Task<Appointment> AddPetAppointmentAsync(Appointment appointment);

        IEnumerable<Immunization> GetPetImmunizations(int petId);
        IEnumerable<Medication> GetPetMedications(int petId);
        IEnumerable<Appointment> GetPetAppointments(int petId);
    }
}
