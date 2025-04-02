using PetHaven.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByReferenceAsync(string reference, bool includeOrder = false);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
    }
}