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
    public class UserRepository: IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<(List<User> Users, int TotalCount)> GetPaginatedUsersAsync(int pageIndex, int pageSize, string searchTerm = null, string role = null, string sortBy = "lastName", string sortDirection = "asc")
        {
            IQueryable<User> query = _context.Users;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.ZipCode.Contains(searchTerm));
            }

            // Apply role filter if provided
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.Role == role);
            }

            // Get total count before pagination
            int totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, sortBy, sortDirection);

            // Apply pagination
            List<User> users = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, string sortBy, string sortDirection)
        {
            // Default sort
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                sortBy = "lastName";
            }

            bool isAscending = string.IsNullOrWhiteSpace(sortDirection) ||
                               sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "firstname" => isAscending
                    ? query.OrderBy(u => u.FirstName)
                    : query.OrderByDescending(u => u.FirstName),
                "lastname" => isAscending
                    ? query.OrderBy(u => u.LastName)
                    : query.OrderByDescending(u => u.LastName),
                "email" => isAscending
                    ? query.OrderBy(u => u.Email)
                    : query.OrderByDescending(u => u.Email),
                "zipcode" => isAscending
                    ? query.OrderBy(u => u.ZipCode)
                    : query.OrderByDescending(u => u.ZipCode),
                "role" => isAscending
                    ? query.OrderBy(u => u.Role)
                    : query.OrderByDescending(u => u.Role),
                "phonenumber" => isAscending
                    ? query.OrderBy(u => u.PhoneNumber)
                    : query.OrderByDescending(u => u.PhoneNumber),
                _ => isAscending
                    ? query.OrderBy(u => u.LastName)
                    : query.OrderByDescending(u => u.LastName),
            };
        }
    }
}
