using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }


        public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO updatedUser)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return false;
            }

            existingUser.FirstName = updatedUser.FirstName ?? existingUser.FirstName;
            existingUser.LastName = updatedUser.LastName ?? existingUser.LastName;
            existingUser.ZipCode = updatedUser.ZipCode  ?? existingUser.ZipCode;
            
            await _userRepository.UpdateUserAsync(existingUser);
            return true;
        }
    }
}
