using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.DTOs
{
    public class SignUpDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string ZipCode { get; set; }

        [Required]
        public string Role { get; set; }

        public VeterinarianDetails? VeterinarianDetails { get; set; }
    }

    public class VeterinarianDetails
    {
        public string? LicenseNumber { get; set; }
        public string? ClinicName { get; set; }
        public string? Specialization { get; set; }

    }
}
