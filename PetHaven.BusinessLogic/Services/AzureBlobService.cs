using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetHaven.BusinessLogic.Interfaces;

namespace PetHaven.BusinessLogic.Services
{
    public class AzureBlobService: IAzureBlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureBlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
            _containerName = configuration["AzureBlobStorage:ContainerName"]!;
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string? fileName = null)
        {
            if (_connectionString == "---") // Azure connection string i not yet available
            {
                return "https://www.princeton.edu/sites/default/files/styles/1x_full_2x_half_crop/public/images/2022/02/KOA_Nassau_2697x1517.jpg?itok=Bg2K7j7J";
            }

            if (string.IsNullOrEmpty(fileName))
            {
                var fileExtension = Path.GetExtension(fileName) ?? ".jpg";
                fileName = $"{Guid.NewGuid()}{fileExtension}";
            }

            var blobClient = new BlobClient(_connectionString, _containerName, fileName);

            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "image/jpeg" });

            return blobClient.Uri.ToString();
        }
    }
}
