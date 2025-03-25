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
        Task<Checkup> AddPetCheckupAsync(Checkup checkup);
        Task<Appointment> AddPetAppointmentAsync(Appointment appointment);
    }
}
