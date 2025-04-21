
using PetHaven.Data.Model;

namespace PetHaven.BusinessLogic.Templates
{
    public static class SignupEmailTemplate
    {
        public static string GetTemplate(User user)
        {
            var websiteUrl = "https://petcareportal.com";
            var loginUrl = $"{websiteUrl}/login";

            var roleSpecificContent = user.Role == "Veterinarian"
                ? "<p>As a veterinarian on our platform, you'll be able to:</p>" +
                  "<ul>" +
                  "<li>Manage pet health records</li>" +
                  "<li>Schedule appointments</li>" +
                  "<li>Track medications and immunizations</li>" +
                  "<li>Connect with pet owners in your area</li>" +
                  "</ul>"
                : "<p>As a pet owner on our platform, you'll be able to:</p>" +
                  "<ul>" +
                  "<li>Track your pet's health records</li>" +
                  "<li>Book appointments with veterinarians</li>" +
                  "<li>Receive medication and immunization reminders</li>" +
                  "<li>Connect with other pet owners in your community</li>" +
                  "</ul>";

            var content = $@"
              <div style='border-bottom: 2px solid #e0f5f5; margin-bottom: 20px; padding-bottom: 20px;'>
                <h2 style='color: #1a9b9b; margin: 0; margin-bottom: 15px; font-family: sans-serif; font-size: 24px; font-weight: bold; text-align: center;'>Welcome to PetHaven!</h2>
              </div>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Hello {user.FirstName},</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>Thank you for creating an account on PetHaven! We're excited to have you join our community of pet lovers.</p>
              
              {roleSpecificContent}
              
              <p style='font-family: sans-serif; font-size: 16px; font-weight: bold; margin: 20px 0 10px 0;'>Your account has been successfully created with:</p>
              <div style='background-color: #e0f5f5; border-radius: 6px; padding: 15px; margin-bottom: 20px;'>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0; margin-bottom: 5px;'><strong>Email:</strong> {user.Email}</p>
                <p style='font-family: sans-serif; font-size: 14px; margin: 0;'><strong>Account type:</strong> {user.Role}</p>
              </div>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;'>You can now sign in to your account and start exploring all the features our platform has to offer!</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 30px;'>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
              
              <p style='font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-top: 25px;'>Woof and Meow,<br>The PetHaven Team</p>
            ";

            return BaseEmailTemplate.GetBaseTemplate(content, loginUrl, "Sign In Now");
        }
    }
}
