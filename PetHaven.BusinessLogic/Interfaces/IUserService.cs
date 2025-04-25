using Microsoft.AspNetCore.Http;
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
    public interface IUserService
    {
        IEnumerable<User> GetAllUsers();
        Task<User?> GetUserByIdAsync(int id);

        Task<User?> GetUserByEmailAsync(string email);

        Task<bool> UpdateUserAsync(int id, UpdateUserDTO updatedUser);

        Task<User?> UpdateUserPhotoAsync(int userId, IFormFile photo);

        Task<PaginatedResult<User>> GetPaginatedUsersAsync(int pageIndex, int pageSize, string searchTerm, string role, string sortBy, string sortDirection);

    }
}
