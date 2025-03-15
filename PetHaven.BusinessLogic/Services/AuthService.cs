using Microsoft.AspNetCore.Identity;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;

            return VerifyPasswordHash(user, password, user.PasswordHash);
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            var passwordHash = CreatePasswordHash(user, password);
            user.PasswordHash = passwordHash;
            
            await _userRepository.AddUserAsync(user);
            return user;
        }

        public string CreatePasswordHash(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPasswordHash(User user, string password, string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, passwordHash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
