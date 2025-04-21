
using PetHaven.Data.Model;

namespace PetHaven.BusinessLogic.Templates
{
    public static class ImmunizationEmailTemplate
    {
        public static string GetTemplate(User petOwner, Pet pet, Immunization immunization)
        {
            var petProfileUrl = $"https://petcareportal.com/pets/{pet.Id}";
            var petTypeString = pet.Type.ToString();
            var daysUntilNextDue = (immunization.NextDueDate - DateTime.Today).Days;

            string reminderClass = daysUntilNextDue <= 30 ? "color: #e66a4d; font-weight: bold;" : "color: #007777;";
            string reminderIcon = daysUntilNextDue <= 30 ? "⚠️" : "📅";

            var birthdayInfo = pet.DateOfBirth.HasValue
                ? $"{DateTime.Now.Year - pet.DateOfBirth.Value.Year} years old"
                : "Age unknown";

            var content = $@"
              <div style='border-bottom: 2px solid #e0f5f5; margin-bottom: 20px; padding-bottom: 15px;'>
                <h2 style='color: #1a9b9b; margin: 0; margin-bottom: 15px; font-family: sans-serif; font-size: 22px; font-weight: bold; text-align: center;'>New Immunization Record for {pet.Name}</h2>
              </div>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Hello {petOwner.FirstName},</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>A new immunization record has been added to your {petTypeString.ToLower()}'s health profile. Here are the details:</p>
              
              <div style='background-color: #e0f5f5; border-radius: 6px; padding: 15px; margin-bottom: 20px;'>
                <div style='display: flex; align-items: center; margin-bottom: 10px;'>
                  <div style='margin-right: 15px;'>
                    <img src='{(string.IsNullOrEmpty(pet.PhotoUrl) ? "https://petcareportal.com/assets/default-pet.png" : pet.PhotoUrl)}' 
                         alt='{pet.Name}' 
                         style='width: 60px; height: 60px; border-radius: 50%; object-fit: cover; border: 2px solid #1a9b9b;'>
                  </div>
                  <div>
                    <h3 style='color: #007777; margin: 0; margin-bottom: 5px; font-family: sans-serif; font-size: 18px; font-weight: bold;'>{pet.Name}</h3>
                    <p style='font-family: sans-serif; font-size: 14px; margin: 0; color: #6c757d;'>{petTypeString} • {pet.Breed} • {birthdayInfo}</p>
                  </div>
                </div>
                
                <div style='border-top: 1px solid #1a9b9b; margin: 10px 0; padding-top: 10px;'>
                  <h3 style='color: #007777; margin: 0; margin-bottom: 15px; font-family: sans-serif; font-size: 18px; font-weight: bold;'>Immunization Details</h3>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Vaccine:</strong> {immunization.Vaccine}</p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Date Administered:</strong> {immunization.DateAdministered.ToString("MMMM dd, yyyy")}</p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Next Due Date:</strong> <span style='{reminderClass}'>{reminderIcon} {immunization.NextDueDate.ToString("MMMM dd, yyyy")} (in {daysUntilNextDue} days)</span></p>
                </div>
              </div>
              
              <div style='border-left: 4px solid #ff7e5f; padding-left: 15px; margin-bottom: 20px;'>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Additional Notes:</strong></p>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0;'>{(string.IsNullOrEmpty(immunization.Notes) ? "No additional notes provided." : immunization.Notes)}</p>
              </div>
              
              <div style='background-color: #f8f9fa; border-radius: 6px; padding: 15px; margin-bottom: 20px;'>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0;'>
                  <strong>💡 Did you know?</strong> Regular immunizations are essential for your pet's health and help protect against serious diseases. Make sure to keep up with {pet.Name}'s vaccination schedule!
                </p>
              </div>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>We'll send you a reminder when the next vaccination is due. You can view your pet's complete immunization records by clicking the button below.</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 30px 0 0 0;'>Warm regards,<br>The PetHaven Team</p>
            ";

            return BaseEmailTemplate.GetBaseTemplate(content, petProfileUrl, "View Pet Profile");
        }
    }
}
