using PetHaven.API.Data;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class UserService: IUserService
    {
        private readonly List<User> _users;
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public User? GetUser(string email)
        {
            return _dbContext.Users.SingleOrDefault(u => u.Email == email);
        }
    }
}
