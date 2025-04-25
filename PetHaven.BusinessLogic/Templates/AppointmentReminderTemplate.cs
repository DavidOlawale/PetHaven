using PetHaven.Data.Model;

namespace PetHaven.BusinessLogic.Templates
{
    public static class AppointmentReminderTemplate
    {
        public static string GetTemplate(User petOwner, Pet pet, Appointment appointment)
        {
            var petProfileUrl = pet.PhotoUrl;
            var petTypeString = pet.Type.ToString();
            
            var timeUntilAppointment = appointment.ScheduledDate - DateTime.Now;
            var hoursLeft = timeUntilAppointment.TotalHours;
            
            string urgencyClass = hoursLeft <= 6 ? "color: #e66a4d; font-weight: bold;" : "color: #007777;";
            string urgencyIcon = hoursLeft <= 6 ? "⚠️" : "🕒";
            
            var birthdayInfo = pet.DateOfBirth.HasValue
                ? $"{DateTime.Now.Year - pet.DateOfBirth.Value.Year} years old"
                : "Age unknown";

            var content = $@"
              <div style='border-bottom: 2px solid #e0f5f5; margin-bottom: 20px; padding-bottom: 15px;'>
                <h2 style='color: #1a9b9b; margin: 0; margin-bottom: 15px; font-family: sans-serif; font-size: 22px; font-weight: bold; text-align: center;'>Appointment Reminder for {pet.Name}</h2>
              </div>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Hello {petOwner.FirstName},</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>This is a friendly reminder about your upcoming appointment for your {petTypeString.ToLower()}. Here are the details:</p>
              
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
                  <h3 style='color: #007777; margin: 0; margin-bottom: 15px; font-family: sans-serif; font-size: 18px; font-weight: bold;'>Appointment Details</h3>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Type:</strong> {appointment.AppointmentType}</p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Date:</strong> {appointment.ScheduledDate.ToString("MMMM dd, yyyy")}</p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Time:</strong> <span style='{urgencyClass}'>{urgencyIcon} {appointment.ScheduledDate.ToString("h:mm tt")}</span></p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Veterinarian:</strong> {appointment.Veterinarian}</p>
                  <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Venue:</strong> {appointment.Venue}</p>
                </div>
              </div>
              
              <div style='border-left: 4px solid #ff7e5f; padding-left: 15px; margin-bottom: 20px;'>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Reason for Visit:</strong></p>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0;'>{(string.IsNullOrEmpty(appointment.Reason) ? "No specific reason provided." : appointment.Reason)}</p>
              </div>
              
              <div style='background-color: #f8f9fa; border-radius: 6px; padding: 15px; margin-bottom: 20px;'>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0;'>
                  <strong>💡 Reminder:</strong> Please arrive 10 minutes before your scheduled appointment time. If you need to reschedule, please contact us as soon as possible.
                </p>
              </div>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>You can view your pet's complete medical history by clicking the button below.</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 30px 0 0 0;'>Warm regards,<br>The PetHaven Team</p>
            ";

            return BaseEmailTemplate.GetBaseTemplate(content, petProfileUrl, "View Pet Profile");
        }
    }
}