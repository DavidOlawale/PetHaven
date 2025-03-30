using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pet?> GetPetByIdAsync(int id)
        {
            return await _context.Pets.FindAsync(id);
        }

        public async Task<List<Pet>> GetPetsByOwnerIdAsync(int ownerId)
        {
            return await _context.Pets.Where(p => p.OwnerId == ownerId).ToListAsync();
        }

        public async Task<Pet> AddPetAsync(Pet pet)
        {
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            return pet;
        }

        public async Task UpdatePetAsync(Pet pet)
        {
            _context.Pets.Update(pet);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePetAsync(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Immunization> AddPetImmunizationAsync(Immunization immunization)
        {
            _context.Immunizations.Update(immunization);
            await _context.SaveChangesAsync();
            return immunization;
        }

        public async Task<Medication> AddPetMedicationAsync(Medication checkup)
        {
            _context.Medications.Update(checkup);
            await _context.SaveChangesAsync();
            return checkup;
        }
        public async Task<Appointment> AddPetAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public IEnumerable<Immunization> GetPetImmunizations(int petId)
        {
            return _context.Immunizations.Where(i => i.PetId == petId).ToList();
        }
        public IEnumerable<Medication> GetPetMedications(int petId)
        {
            return _context.Medications.Where(i => i.PetId == petId).ToList();
        }
        public IEnumerable<Appointment> GetPetAppointments(int petId)
        {
            return _context.Appointments.Where(i => i.PetId == petId).ToList();
        }
    }
}
