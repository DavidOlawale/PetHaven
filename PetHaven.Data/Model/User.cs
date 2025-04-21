using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class User: BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string ZipCode { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public string? LicenseNumber { get; set; }
        public string? ClinicName { get; set; }
        public string? Specialization { get; set; }
    }
}
