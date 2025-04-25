//using Firebase.Storage;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.IO;
//using System.Threading.Tasks;
//using PetHaven.BusinessLogic.Interfaces;

//namespace PetHaven.BusinessLogic.Services
//{
//    public class FirebaseBlobService : IAzureBlobService
//    {
//        private readonly string _storageBucket;
//        private readonly string _apiKey;

//        public FirebaseBlobService(IConfiguration configuration)
//        {
//            _storageBucket = configuration["FirebaseStorage:StorageBucket"]!;
//            _apiKey = configuration["FirebaseStorage:ApiKey"]!;
//        }

//        public virtual async Task<string> UploadImageAsync(Stream fileStream, string? fileName = null)
//        {
//            if (string.IsNullOrEmpty(_storageBucket)) // Firebase not yet configured
//            {
//                return "https://www.princeton.edu/sites/default/files/styles/1x_full_2x_half_crop/public/images/2022/02/KOA_Nassau_2697x1517.jpg?itok=Bg2K7j7J";
//            }

//            if (string.IsNullOrEmpty(fileName))
//            {
//                var fileExtension = ".jpg";
//                fileName = $"{Guid.NewGuid()}{fileExtension}";
//            }

//            // Create a Firebase Storage instance
//            var storage = new FirebaseStorage(_storageBucket);

//            // Upload the file
//            var task = await storage
//                .Child("images")
//                .Child(fileName)
//                .PutAsync(fileStream);

//            // Return the download URL
//            return task;
//        }

//        public async Task<string> UploadImageAsync(string image, string? fileName = null)
//        {
//            // Remove the prefix (e.g., "data:image/jpeg;base64,")
//            var base64Data = image.Split(',')[1];
//            // Convert Base64 to byte array
//            var imageBytes = Convert.FromBase64String(base64Data);
//            var imageStream = new MemoryStream(imageBytes);
//            return await UploadImageAsync(imageStream, fileName);
//        }
//    }
//}