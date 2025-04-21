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
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                await _context.Entry(pet).Reference(p => p.Owner).LoadAsync();
            }
            return pet;
        }

        public IEnumerable<Pet> GetAllPets()
        {
            return _context.Pets.Include(p => p.Owner).ToList();
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
            _context.Immunizations.Add(immunization);
            await _context.SaveChangesAsync();
            return immunization;
        }

        public async Task<Medication> AddPetMedicationAsync(Medication checkup)
        {
            _context.Medications.Add(checkup);
            await _context.SaveChangesAsync();
            return checkup;
        }
        public async Task<Appointment> AddPetAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
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

        public IEnumerable<Appointment> GetAllPetAppointments()
        {
            return _context.Appointments.ToList();
        }


        public async Task<Immunization> UpdatePetImmunization(int immunizationId, Immunization immunization)
        {
            var dbImmunization = _context.Immunizations.Find(immunizationId);
            dbImmunization.NextDueDate = immunization.NextDueDate;
            dbImmunization.DateAdministered = immunization.DateAdministered;
            dbImmunization.Vaccine = immunization.Vaccine;
            dbImmunization.Notes = immunization.Notes;

            _context.Immunizations.Update(dbImmunization);
            await _context.SaveChangesAsync();
            return dbImmunization;


        }
        public async Task<Medication> UpdatePetMedication(int medicationId, Medication medication)
        {
            var dbMedication = _context.Medications.Find(medicationId);
            dbMedication.Name= medication.Name;
            dbMedication.Dosage = medication.Dosage;
            dbMedication.Frequency = medication.Frequency;
            dbMedication.StartDate = medication.StartDate;
            dbMedication.EndDate = medication.EndDate;
            dbMedication.Notes = medication.Notes;

            _context.Medications.Update(dbMedication);
            await _context.SaveChangesAsync();
            return dbMedication;
        }
        public async Task<Appointment> UpdatePetAppointment(int appointmentId, Appointment appointment)
        {
            var dbAppointment = _context.Appointments.Find(appointmentId);
            dbAppointment.AppointmentType = appointment.AppointmentType;
            dbAppointment.ScheduledDate = appointment.ScheduledDate;
            dbAppointment.Veterinarian = appointment.Veterinarian;
            dbAppointment.Venue = appointment.Venue;
            dbAppointment.Notes = appointment.Notes;

            _context.Appointments.Update(dbAppointment);
            await _context.SaveChangesAsync();
            return dbAppointment;
        }


        public async Task DeletePetImmunization(int immunizationId)
        {
            var immunization = _context.Immunizations.Find(immunizationId);
            _context.Immunizations.Remove(immunization);
            await _context.SaveChangesAsync();
        }
        public async Task DeletePetMedication(int medicationId)
        {
            var medication = _context.Medications.Find(medicationId);
            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();
        }
        public async Task DeletePetAppointment(int appointmentId)
        {
            var appointment = _context.Appointments.Find(appointmentId);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }
}
