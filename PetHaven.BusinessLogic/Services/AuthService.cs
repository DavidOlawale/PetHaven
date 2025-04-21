using Microsoft.AspNetCore.Identity;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.BusinessLogic.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _emailService = emailService;
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;

            return VerifyPasswordHash(user, password, user.PasswordHash);
        }

        public async Task<User> RegisterAsync(SignUpDTO signUpDTO)
        {
            // Validate User role
            if (signUpDTO.Role!= UserRoles.PetOwner && signUpDTO.Role != UserRoles.Veterinarian)
            {
                throw new Exception("Invalid role name");
            }
            var user = new User
            {
                FirstName = signUpDTO.FirstName,
                LastName = signUpDTO.LastName,
                Email = signUpDTO.Email,
                ZipCode = signUpDTO.ZipCode,
                Role = signUpDTO.Role
            };

            if (signUpDTO.Role == UserRoles.Veterinarian)
            {
                user.LicenseNumber = signUpDTO?.VeterinarianDetails?.LicenseNumber;
                user.ClinicName = signUpDTO?.VeterinarianDetails?.ClinicName;
                user.Specialization = signUpDTO?.VeterinarianDetails?.Specialization;
            }
            if ((await _userRepository.GetUserByEmailAsync(user.Email)) != null){
                throw new Exception("Email already exists");
            }

            var passwordHash = CreatePasswordHash(user, signUpDTO.Password);
            user.PasswordHash = passwordHash;
            
            await _userRepository.AddUserAsync(user);
            await _emailService.SendSignupConfirmationAsync(user);

            return user;
        }

        public async Task<bool> CheckEmailExists(string email)
        {
            return (await _userRepository.GetUserByEmailAsync(email)) != null;
        }

        public virtual string CreatePasswordHash(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public virtual bool VerifyPasswordHash(User user, string password, string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, passwordHash, password);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
