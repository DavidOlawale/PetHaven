using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.Data.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByReferenceAsync(string reference, bool includeOrder = false)
        {
            var query = _context.Payments.AsQueryable();

            if (includeOrder)
                query = query.Include(p => p.Order);

            return await query.FirstOrDefaultAsync(p => p.Reference == reference);
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
