using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.BusinessLogic.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBlobService _blobService;

        public UserService(IUserRepository userRepository, IBlobService azureBlobService   )
        {
            _userRepository = userRepository;
            _blobService = azureBlobService;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
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
            existingUser.PhoneNumber = updatedUser.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.ZipCode = updatedUser.ZipCode  ?? existingUser.ZipCode;
            
            await _userRepository.UpdateUserAsync(existingUser);
            return true;
        }

        public async Task<User?> UpdateUserPhotoAsync(int userId, IFormFile photo)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }



            var fileStream = photo.OpenReadStream();
            var imageUrl = await _blobService.UploadImageAsync(fileStream);
            user.photoUrl = imageUrl;

            await _userRepository.UpdateUserAsync(user);
            return user;
        }

        public async Task<PaginatedResult<User>> GetPaginatedUsersAsync( int pageIndex, int pageSize, string searchTerm = null, string role = null, string sortBy = "lastName", string sortDirection = "asc")
        {
            // Ensure positive values for pagination
            pageIndex = Math.Max(0, pageIndex);
            pageSize = Math.Max(1, pageSize);

            var (users, totalCount) = await _userRepository.GetPaginatedUsersAsync(
                pageIndex, pageSize, searchTerm, role, sortBy, sortDirection);

            return new PaginatedResult<User>
            {
                Items = users,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
