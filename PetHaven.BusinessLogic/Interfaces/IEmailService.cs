using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Interfaces
{
    public interface IEmailService
    {
        Task SendSignupConfirmationAsync(User user);
        Task SendMedicationNotificationAsync(User petOwner, Pet pet, Medication medication);
        Task SendImmunizationNotificationAsync(User petOwner, Pet pet, Immunization immunization);
    }
}
