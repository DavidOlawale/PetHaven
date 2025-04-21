//using Azure.Storage;
//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Models;
//using Microsoft.Extensions.Configuration;
//using Moq;
//using PetHaven.BusinessLogic.Services;
//using System;
//using System.IO;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace PetHaven.Tests.Services
//{
//    public class AzureBlobServiceTests
//    {
//        private readonly Mock<IConfiguration> _mockConfiguration;
//        private readonly Mock<IConfigurationSection> _mockConnectionStringSection;
//        private readonly Mock<IConfigurationSection> _mockContainerNameSection;

//        public AzureBlobServiceTests()
//        {
//            _mockConfiguration = new Mock<IConfiguration>();
//            _mockConnectionStringSection = new Mock<IConfigurationSection>();
//            _mockContainerNameSection = new Mock<IConfigurationSection>();

//            // Setup configurations
//            _mockConnectionStringSection.Setup(x => x.Value).Returns("test-connection-string");
//            _mockContainerNameSection.Setup(x => x.Value).Returns("test-container");

//            _mockConfiguration.Setup(c => c["AzureBlobStorage:ConnectionString"])
//                .Returns(_mockConnectionStringSection.Object.Value);
//            _mockConfiguration.Setup(c => c["AzureBlobStorage:ContainerName"])
//                .Returns(_mockContainerNameSection.Object.Value);
//        }


//        [Fact]
//        public async Task UploadImageAsync_Stream_GeneratesGuidFileName_WhenFileNameIsNull()
//        {
//            // Arrange
//            var service = CreateServiceWithMockedBlobClient(out var mockBlobClient);
//            var fileStream = new MemoryStream();

//            // Act
//            var result = await service.UploadImageAsync(fileStream);

//            // Assert
//            mockBlobClient.Verify();
//            Assert.Contains(mockBlobClient.Object.Uri.ToString(), result);
//        }

//        [Fact]
//        public async Task UploadImageAsync_Stream_UsesProvidedFileName()
//        {
//            // Arrange
//            var service = CreateServiceWithMockedBlobClient(out var mockBlobClient);
//            var fileStream = new MemoryStream();
//            var fileName = "test-image.jpg";

//            // Act
//            var result = await service.UploadImageAsync(fileStream, fileName);

//            // Assert
//            mockBlobClient.Verify();
//            Assert.Contains(mockBlobClient.Object.Uri.ToString(), result);
//        }

//        [Fact]
//        public async Task UploadImageAsync_Base64String_ParsesAndUploadsImage()
//        {
//            // Arrange
//            var service = CreateServiceWithMockedBlobClient(out var mockBlobClient);
//            // A minimal valid base64 image string
//            var base64Image = "data:image/jpeg;base64,/9j";

//            // Act
//            var result = await service.UploadImageAsync(base64Image, "test-base64.jpg");

//            // Assert
//            mockBlobClient.Verify();
//            Assert.Contains(mockBlobClient.Object.Uri.ToString(), result);
//        }

//        private AzureBlobService CreateServiceWithMockedBlobClient(out Mock<BlobClient> mockBlobClient)
//        {
//            // Create a service that will use our mocked configuration
//            var service = new AzureBlobService(_mockConfiguration.Object);

//            // Setup BlobClient mock
//            mockBlobClient = new Mock<BlobClient>();
//            mockBlobClient.Setup(b => b.Uri).Returns(new Uri("https://test.blob.core.windows.net/test-container/test-image.jpg"));

//            // We need to mock the Upload method
//            mockBlobClient
//                .Setup(b => b.UploadAsync(
//                    It.IsAny<Stream>(),
//                    It.IsAny<BlobHttpHeaders>()
//                    ))
//                .ReturnsAsync(new Mock<Azure.Response<BlobContentInfo>>().Object)
//                .Verifiable();

//            // Use reflection to replace the BlobClient creation
//            // Note: This is a bit of a hack, but it allows us to test without actually connecting to Azure
//            var blobClientConstructor = typeof(BlobClient).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];

//            // Intercept the BlobClient creation
//            var originalBlobClientCreator = typeof(BlobClient).GetConstructor(
//                BindingFlags.Public | BindingFlags.Instance,
//                null,
//                new[] { typeof(string), typeof(string), typeof(string) },
//                null);

//            // Replace the method
//            MethodInfo methodToReplace = typeof(BlobClient).GetMethod("Create",
//                BindingFlags.Public | BindingFlags.Static,
//                null,
//                new[] { typeof(string), typeof(string), typeof(string) },
//                null);

//            if (methodToReplace == null)
//            {
//                // If we can't find the specific method, return the service and let it use the real BlobClient
//                // The test will fail anyway, but at least we tried
//                return service;
//            }

//            // Return our mocked BlobClient
//            var blobClientCreatorField = typeof(BlobClient).GetField(
//                "Create",
//                BindingFlags.Static | BindingFlags.NonPublic);

//            if (blobClientCreatorField != null)
//            {
//                // Set the field to return our mocked client
//                // This is a simplified approach and might not work in all versions of the Azure SDK
//                blobClientCreatorField.SetValue(null, (Func<string, string, string, BlobClient>)((connString, containerName, blobName) => mockBlobClient.Object));
//            }

//            return service;
//        }
//    }
//}