using Microsoft.AspNetCore.Http;
using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.DTOs
{
    public class AddPetDTO
    {
        public string Name { get; set; }
        public PetType Type { get; set; }
        public string Breed { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Weight { get; set; }
        public string? Allergies { get; set; }
        public int? OwnerID { get; set; }
        public IFormFile Photo { get; set; }
    }
}
