using PetHaven.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories.Interfaces
{
    public interface IReminderRepository
    {
        Task<Reminder?> GetReminderByIdAsync(int id);
        Task<List<Reminder>> GetRemindersByPetIdAsync(int petId);
        Task<Reminder> AddReminderAsync(Reminder reminder);
        Task UpdateReminderAsync(Reminder reminder);
        Task DeleteReminderAsync(int id);
    }
}