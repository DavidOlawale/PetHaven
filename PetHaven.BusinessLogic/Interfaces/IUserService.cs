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
        Task<User?> GetUserByIdAsync(int id);

        Task<User?> GetUserByEmailAsync(string email);

        Task<bool> UpdateUserAsync(int id, UpdateUserDTO updatedUser);

    }
}
