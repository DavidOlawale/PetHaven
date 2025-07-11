﻿using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        Task<(List<User> Users, int TotalCount)> GetPaginatedUsersAsync(
        int pageIndex,
        int pageSize,
        string searchTerm,
        string role,
        string sortBy = "lastName",
        string sortDirection = "asc");

        Task<User?> GetUserByIdAsync(int id);

        Task<User?> GetUserByEmailAsync(string email);

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);
    }
}
