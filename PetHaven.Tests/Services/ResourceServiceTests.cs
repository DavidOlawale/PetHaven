using Moq;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using Xunit;
namespace PetHaven.Tests.Services
{
    public class ResourceServiceTests
    {
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly ResourceService _service;

        public ResourceServiceTests()
        {
            _mockResourceRepository = new Mock<IResourceRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _service = new ResourceService(_mockResourceRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public async Task GetAllResourcesAsync_ReturnsAllResources()
        {
            // Arrange
            var expectedResources = new List<Resource>
            {
                new Resource
                {
                    Id = 1,
                    Title = "Pet Care Basics",
                    Content = "Essential pet care information...",
                    PublishedDate = DateTime.UtcNow.AddDays(-10),
                    Author = "John Doe",
                    Category = "Care",
                    CreatorId = 1
                },
                new Resource
                {
                    Id = 2,
                    Title = "Dog Training 101",
                    Content = "Basic dog training techniques...",
                    PublishedDate = DateTime.UtcNow.AddDays(-5),
                    Author = "Jane Smith",
                    Category = "Training",
                    CreatorId = 2
                }
            };

            _mockResourceRepository.Setup(r => r.GetAllResourcesAsync(null))
                .ReturnsAsync(expectedResources);

            // Act
            var result = await _service.GetAllResourcesAsync();

            // Assert
            Assert.Equal(expectedResources, result);
            _mockResourceRepository.Verify(r => r.GetAllResourcesAsync(null), Times.Once);
        }

        [Fact]
        public async Task GetAllResourcesAsync_WithCategory_ReturnsFilteredResources()
        {
            // Arrange
            string category = "Training";
            var expectedResources = new List<Resource>
            {
                new Resource
                {
                    Id = 2,
                    Title = "Dog Training 101",
                    Content = "Basic dog training techniques...",
                    PublishedDate = DateTime.UtcNow.AddDays(-5),
                    Author = "Jane Smith",
                    Category = "Training",
                    CreatorId = 2
                }
            };

            _mockResourceRepository.Setup(r => r.GetAllResourcesAsync(category))
                .ReturnsAsync(expectedResources);

            // Act
            var result = await _service.GetAllResourcesAsync(category);

            // Assert
            Assert.Equal(expectedResources, result);
            _mockResourceRepository.Verify(r => r.GetAllResourcesAsync(category), Times.Once);
        }

        [Fact]
        public async Task GetResourceByIdAsync_ReturnsResource_WhenResourceExists()
        {
            // Arrange
            int resourceId = 1;
            var expectedResource = new Resource
            {
                Id = resourceId,
                Title = "Pet Care Basics",
                Content = "Essential pet care information...",
                PublishedDate = DateTime.UtcNow.AddDays(-10),
                Author = "John Doe",
                Category = "Care",
                CreatorId = 1
            };

            _mockResourceRepository.Setup(r => r.GetResourceByIdAsync(resourceId))
                .ReturnsAsync(expectedResource);

            // Act
            var result = await _service.GetResourceByIdAsync(resourceId);

            // Assert
            Assert.Equal(expectedResource, result);
            _mockResourceRepository.Verify(r => r.GetResourceByIdAsync(resourceId), Times.Once);
        }

        [Fact]
        public async Task GetResourceByIdAsync_ReturnsNull_WhenResourceDoesNotExist()
        {
            // Arrange
            int resourceId = 999;
            _mockResourceRepository.Setup(r => r.GetResourceByIdAsync(resourceId))
                .ReturnsAsync((Resource)null);

            // Act
            var result = await _service.GetResourceByIdAsync(resourceId);

            // Assert
            Assert.Null(result);
            _mockResourceRepository.Verify(r => r.GetResourceByIdAsync(resourceId), Times.Once);
        }

        [Fact]
        public async Task CreateResourceAsync_SetsPublishedDateAndAuthorAndCreatesResource()
        {
            // Arrange
            var creatorId = 1;
            var user = new User
            {
                Id = creatorId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var resourceToCreate = new Resource
            {
                Title = "New Pet Nutrition Guide",
                Content = "Comprehensive guide to pet nutrition...",
                Category = "Nutrition",
                CreatorId = creatorId
            };

            Resource capturedResource = null;
            var createdResource = new Resource
            {
                Id = 3,
                Title = resourceToCreate.Title,
                Content = resourceToCreate.Content,
                Category = resourceToCreate.Category,
                CreatorId = resourceToCreate.CreatorId,
                PublishedDate = DateTime.UtcNow,
                Author = "John Doe"
            };

            _mockUserRepository.Setup(r => r.GetUserByIdAsync(creatorId))
                .ReturnsAsync(user);

            _mockResourceRepository.Setup(r => r.AddResourceAsync(It.IsAny<Resource>()))
                .Callback<Resource>(r => capturedResource = r)
                .ReturnsAsync(createdResource);

            // Act
            var result = await _service.CreateResourceAsync(resourceToCreate);

            // Assert
            Assert.Equal(createdResource, result);

            // Verify resource was created with correct properties
            Assert.NotNull(capturedResource);
            Assert.Equal(resourceToCreate.Title, capturedResource.Title);
            Assert.Equal(resourceToCreate.Content, capturedResource.Content);
            Assert.Equal(resourceToCreate.Category, capturedResource.Category);
            Assert.Equal(resourceToCreate.CreatorId, capturedResource.CreatorId);

            // Verify Author was set correctly
            Assert.Equal("John Doe", capturedResource.Author);

            // Verify PublishedDate was set
            Assert.NotEqual(default, capturedResource.PublishedDate);

            // Check that the PublishedDate is within the last few seconds
            var timeDiff = DateTime.UtcNow - capturedResource.PublishedDate;
            Assert.True(timeDiff.TotalSeconds < 5);

            _mockUserRepository.Verify(r => r.GetUserByIdAsync(creatorId), Times.Once);
            _mockResourceRepository.Verify(r => r.AddResourceAsync(It.IsAny<Resource>()), Times.Once);
        }

        [Fact]
        public async Task UpdateResourceAsync_UpdatesResource()
        {
            // Arrange
            var resourceToUpdate = new Resource
            {
                Id = 1,
                Title = "Updated Pet Care Basics",
                Content = "Updated essential pet care information...",
                PublishedDate = DateTime.UtcNow.AddDays(-9),
                Author = "John Doe",
                Category = "Care",
                CreatorId = 1
            };

            _mockResourceRepository.Setup(r => r.UpdateResourceAsync(resourceToUpdate))
                .ReturnsAsync(resourceToUpdate);

            // Act
            var result = await _service.UpdateResourceAsync(resourceToUpdate);

            // Assert
            Assert.Equal(resourceToUpdate, result);
            _mockResourceRepository.Verify(r => r.UpdateResourceAsync(resourceToUpdate), Times.Once);
        }

        [Fact]
        public async Task DeleteResourceAsync_DeletesResource()
        {
            // Arrange
            int resourceId = 1;
            _mockResourceRepository.Setup(r => r.DeleteResourceAsync(resourceId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteResourceAsync(resourceId);

            // Assert
            _mockResourceRepository.Verify(r => r.DeleteResourceAsync(resourceId), Times.Once);
        }
    }
}