using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Interfaces
{
    public interface IAzureBlobService
    {
        Task<string> UploadImageAsync(Stream fileStream, string? fileName = null);
        Task<string> UploadImageAsync(string image, string? fileName = null);
    }
}
