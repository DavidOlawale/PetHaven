﻿using PetHaven.BusinessLogic.DTOs;
using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Interfaces
{
    public interface IAuthService
    {
        Task<bool> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(SignUpDTO signUpDTO);
        string CreatePasswordHash(User user, string password);
        bool VerifyPasswordHash(User user, string password, string storedHash);
        Task<bool> CheckEmailExists(string email);
    }
}
