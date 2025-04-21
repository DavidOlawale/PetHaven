
using PetHaven.Data.Model;

namespace PetHaven.BusinessLogic.Templates
{
    public static class MedicationEmailTemplate
    {
        public static string GetTemplate(User petOwner, Pet pet, Medication medication)
        {
            var petProfileUrl = $"https://petcareportal.com/pets/{pet.Id}";
            var petTypeString = pet.Type.ToString();

            var endDateInfo = medication.EndDate.HasValue
                ? $"<p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>End Date:</strong> {medication.EndDate.Value.ToString("MMMM dd, yyyy")}</p>"
                : "<p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>End Date:</strong> Ongoing</p>";

            var content = $@"
              <div style='border-bottom: 2px solid #e0f5f5; margin-bottom: 20px; padding-bottom: 15px;'>
                <h2 style='color: #1a9b9b; margin: 0; margin-bottom: 15px; font-family: sans-serif; font-size: 22px; font-weight: bold; text-align: center;'>New Medication Added for {pet.Name}</h2>
              </div>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Hello {petOwner.FirstName},</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>A new medication has been added to your {petTypeString.ToLower()}'s health record. Here are the details:</p>
              
              <div style='background-color: #e0f5f5; border-radius: 6px; padding: 15px; margin-bottom: 20px;'>
                <div style='display: flex; align-items: center; margin-bottom: 10px;'>
                  <div style='margin-right: 15px;'>
                    <img src='{(string.IsNullOrEmpty(pet.PhotoUrl) ? "https://petcareportal.com/assets/default-pet.png" : pet.PhotoUrl)}' 
                         alt='{pet.Name}' 
                         style='width: 60px; height: 60px; border-radius: 50%; object-fit: cover; border: 2px solid #1a9b9b;'>
                  </div>
                  <div>
                    <h3 style='color: #007777; margin: 0; margin-bottom: 5px; font-family: sans-serif; font-size: 18px; font-weight: bold;'>{pet.Name}</h3>
                    <p style='font-family: sans-serif; font-size: 14px; margin: 0; color: #6c757d;'>{petTypeString} • {pet.Breed}</p>
                  </div>
                </div>
                
                <div style='border-top: 1px solid #1a9b9b; margin: 10px 0; padding-top: 10px;'>
                  <h3 style='color: #007777; margin: 0; margin-bottom: 15px; font-family: sans-serif; font-size: 18px; font-weight: bold;'>Medication Details</h3>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Medication:</strong> {medication.Name}</p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Dosage:</strong> {medication.Dosage}</p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Frequency:</strong> {medication.Frequency}</p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Start Date:</strong> {medication.StartDate.ToString("MMMM dd, yyyy")}</p>
                  {endDateInfo}
                </div>
              </div>
              
              <div style='border-left: 4px solid #ff7e5f; padding-left: 15px; margin-bottom: 20px;'>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Additional Notes:</strong></p>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0;'>{(string.IsNullOrEmpty(medication.Notes) ? "No additional notes provided." : medication.Notes)}</p>
              </div>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>It's important to follow the prescribed medication schedule. You can view {pet.Name}'s complete health record by clicking the button below.</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 30px 0 0 0;'>Warm regards,<br>The PetHaven Team</p>
            ";

            return BaseEmailTemplate.GetBaseTemplate(content, petProfileUrl, "View Pet Profile");
        }
    }
}
