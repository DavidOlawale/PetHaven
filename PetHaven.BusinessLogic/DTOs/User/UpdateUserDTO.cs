using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.DTOs.User
{
    public class UpdateUserDTO
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }

        public string? ZipCode { get; set; }

    }
}
