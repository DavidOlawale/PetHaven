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
        Task<Checkup> AddPetCheckupAsync(Checkup checkup);
        Task<Appointment> AddPetAppointmentAsync(Appointment appointment);
    }
}
