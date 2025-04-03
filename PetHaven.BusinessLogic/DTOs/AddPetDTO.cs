using Microsoft.AspNetCore.Http;
using PetHaven.Data.Model;
using System;

namespace PetHaven.BusinessLogic.DTOs
{
    public class AddPetDTO
    {
        public string Name { get; set; }
        public PetType Type { get; set; }
        public string Breed { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double Weight { get; set; }
        public string? Allergies { get; set; }
        public int? OwnerID { get; set; }
        public IFormFile Photo { get; set; }
    }
}
