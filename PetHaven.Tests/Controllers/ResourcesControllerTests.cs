using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class ResourcesControllerTests
    {
        private readonly Mock<IResourceService> _mockResourceService;
        private readonly ResourcesController _controller;
        private readonly int _testUserId = 5;

        public ResourcesControllerTests()
        {
            _mockResourceService = new Mock<IResourceService>();
            _controller = new ResourcesController(_mockResourceService.Object);

            // Simulate authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Setup ControllerContext with HttpContext
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithAllResources()
        {
            // Arrange
            var expectedResources = new List<Resource>
            {
                new Resource
                {
                    Id = 1,
                    Title = "Pet Care Basics",
                    Content = "Essential pet care information...",
                    PublishedDate = DateTime.Now.AddDays(-10),
                    Author = "John Doe",
                    Category = "Care",
                    CreatorId = 1
                },
                new Resource
                {
                    Id = 2,
                    Title = "Dog Training 101",
                    Content = "Basic dog training techniques...",
                    PublishedDate = DateTime.Now.AddDays(-5),
                    Author = "Jane Smith",
                    Category = "Training",
                    CreatorId = 2
                }
            };

            _mockResourceService.Setup(s => s.GetAllResourcesAsync())
                .ReturnsAsync(expectedResources);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResources = Assert.IsAssignableFrom<IEnumerable<Resource>>(okResult.Value);
            Assert.Equal(expectedResources, returnedResources);
        }

        [Fact]
        public async Task GetById_ReturnsOkWithResource()
        {
            // Arrange
            int resourceId = 1;
            var expectedResource = new Resource
            {
                Id = resourceId,
                Title = "Pet Care Basics",
                Content = "Essential pet care information...",
                PublishedDate = DateTime.Now.AddDays(-10),
                Author = "John Doe",
                Category = "Care",
                CreatorId = 1
            };

            _mockResourceService.Setup(s => s.GetResourceByIdAsync(resourceId))
                .ReturnsAsync(expectedResource);

            // Act
            var result = await _controller.GetById(resourceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResource = Assert.IsType<Resource>(okResult.Value);
            Assert.Equal(expectedResource, returnedResource);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionWithResource()
        {
            // Arrange
            var newResource = new Resource
            {
                Title = "New Pet Nutrition Guide",
                Content = "Comprehensive guide to pet nutrition...",
                PublishedDate = DateTime.Now,
                Author = "Admin User",
                Category = "Nutrition",
                CreatorId = _testUserId
            };

            var createdResource = new Resource
            {
                Id = 3,
                Title = newResource.Title,
                Content = newResource.Content,
                PublishedDate = newResource.PublishedDate,
                Author = newResource.Author,
                Category = newResource.Category,
                CreatorId = newResource.CreatorId
            };

            _mockResourceService.Setup(s => s.CreateResourceAsync(newResource))
                .ReturnsAsync(createdResource);

            // Act
            var result = await _controller.Create(newResource);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ResourcesController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(createdResource.Id, createdAtActionResult.RouteValues!["id"]);

            var returnedResource = Assert.IsType<Resource>(createdAtActionResult.Value);
            Assert.Equal(createdResource, returnedResource);
        }

        [Fact]
        public async Task Update_ReturnsOkWithUpdatedResource_WhenIdMatches()
        {
            // Arrange
            int resourceId = 1;
            var resourceToUpdate = new Resource
            {
                Id = resourceId,
                Title = "Updated Pet Care Basics",
                Content = "Updated essential pet care information...",
                PublishedDate = DateTime.Now.AddDays(-9),
                Author = "John Doe",
                Category = "Care",
                CreatorId = 1
            };

            _mockResourceService.Setup(s => s.UpdateResourceAsync(resourceToUpdate))
                .ReturnsAsync(resourceToUpdate);

            // Act
            var result = await _controller.Update(resourceId, resourceToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResource = Assert.IsType<Resource>(okResult.Value);
            Assert.Equal(resourceToUpdate, returnedResource);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            int resourceId = 1;
            var resourceToUpdate = new Resource
            {
                Id = 2, // Different from route id
                Title = "Updated Pet Care Basics",
                Content = "Updated essential pet care information...",
                PublishedDate = DateTime.Now.AddDays(-9),
                Author = "John Doe",
                Category = "Care",
                CreatorId = 1
            };

            // Act
            var result = await _controller.Update(resourceId, resourceToUpdate);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mockResourceService.Verify(s => s.UpdateResourceAsync(It.IsAny<Resource>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int resourceId = 1;

            _mockResourceService.Setup(s => s.DeleteResourceAsync(resourceId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(resourceId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockResourceService.Verify(s => s.DeleteResourceAsync(resourceId), Times.Once);
        }
    }
}