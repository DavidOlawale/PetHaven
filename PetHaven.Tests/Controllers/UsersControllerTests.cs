using Microsoft.AspNetCore.Mvc;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using System.Threading.Tasks;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetUsers_WithDefaultParameters_ReturnsOkResultWithPaginatedUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = "User" },
                new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Role = "Administrator" }
            };

            var paginatedResult = new PaginatedResult<User>
            {
                Items = expectedUsers,
                TotalCount = 2,
                PageIndex = 0,
                PageSize = 10
            };

            _mockUserService.Setup(s => s.GetPaginatedUsersAsync(
                    0, 10, null, null, "lastName", "asc"))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResult = Assert.IsType<PaginatedResult<User>>(okResult.Value);

            Assert.Equal(paginatedResult.TotalCount, returnedResult.TotalCount);
            Assert.Equal(paginatedResult.PageIndex, returnedResult.PageIndex);
            Assert.Equal(paginatedResult.PageSize, returnedResult.PageSize);
            Assert.Equal(paginatedResult.Items.Count, returnedResult.Items.Count);

            _mockUserService.Verify(s => s.GetPaginatedUsersAsync(
                0, 10, null, null, "lastName", "asc"), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WithCustomParameters_ReturnsOkResultWithFilteredPaginatedUsers()
        {
            // Arrange
            int page = 1;
            int pageSize = 5;
            string searchTerm = "John";
            string role = "Administrator";
            string sortBy = "firstName";
            string sortDirection = "desc";

            var expectedUsers = new List<User>
            {
                new User { Id = 3, FirstName = "John", LastName = "Smith", Email = "john.smith@example.com", Role = "Administrator" }
            };

            var paginatedResult = new PaginatedResult<User>
            {
                Items = expectedUsers,
                TotalCount = 1,
                PageIndex = page,
                PageSize = pageSize
            };

            _mockUserService.Setup(s => s.GetPaginatedUsersAsync(
                    page, pageSize, searchTerm, role, sortBy, sortDirection))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetUsers(page, pageSize, searchTerm, role, sortBy, sortDirection);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResult = Assert.IsType<PaginatedResult<User>>(okResult.Value);

            Assert.Equal(paginatedResult.TotalCount, returnedResult.TotalCount);
            Assert.Equal(paginatedResult.PageIndex, returnedResult.PageIndex);
            Assert.Equal(paginatedResult.PageSize, returnedResult.PageSize);
            Assert.Equal(paginatedResult.Items.Count, returnedResult.Items.Count);
            Assert.Equal("John", returnedResult.Items.First().FirstName);
            Assert.Equal("Administrator", returnedResult.Items.First().Role);

            _mockUserService.Verify(s => s.GetPaginatedUsersAsync(
                page, pageSize, searchTerm, role, sortBy, sortDirection), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WithEmptyResult_ReturnsOkResultWithEmptyList()
        {
            // Arrange
            var paginatedResult = new PaginatedResult<User>
            {
                Items = new List<User>(),
                TotalCount = 0,
                PageIndex = 0,
                PageSize = 10
            };

            _mockUserService.Setup(s => s.GetPaginatedUsersAsync(
                    0, 10, null, null, "lastName", "asc"))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResult = Assert.IsType<PaginatedResult<User>>(okResult.Value);

            Assert.Empty(returnedResult.Items);
            Assert.Equal(0, returnedResult.TotalCount);

            _mockUserService.Verify(s => s.GetPaginatedUsersAsync(
                0, 10, null, null, "lastName", "asc"), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WithSpecificRoleFilter_ReturnsFilteredUsers()
        {
            // Arrange
            string role = "User";

            var expectedUsers = new List<User>
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = "User" },
                new User { Id = 4, FirstName = "Sarah", LastName = "Jones", Email = "sarah.jones@example.com", Role = "User" }
            };

            var paginatedResult = new PaginatedResult<User>
            {
                Items = expectedUsers,
                TotalCount = 2,
                PageIndex = 0,
                PageSize = 10
            };

            _mockUserService.Setup(s => s.GetPaginatedUsersAsync(
                    0, 10, null, role, "lastName", "asc"))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetUsers(role: role);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResult = Assert.IsType<PaginatedResult<User>>(okResult.Value);

            Assert.Equal(2, returnedResult.Items.Count);
            Assert.All(returnedResult.Items, item => Assert.Equal("User", item.Role));

            _mockUserService.Verify(s => s.GetPaginatedUsersAsync(
                0, 10, null, role, "lastName", "asc"), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WithSearchTerm_ReturnsMatchingUsers()
        {
            // Arrange
            string searchTerm = "doe";

            var expectedUsers = new List<User>
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = "User" }
            };

            var paginatedResult = new PaginatedResult<User>
            {
                Items = expectedUsers,
                TotalCount = 1,
                PageIndex = 0,
                PageSize = 10
            };

            _mockUserService.Setup(s => s.GetPaginatedUsersAsync(
                    0, 10, searchTerm, null, "lastName", "asc"))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _controller.GetUsers(searchTerm: searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResult = Assert.IsType<PaginatedResult<User>>(okResult.Value);

            Assert.Single(returnedResult.Items);
            Assert.Contains("Doe", returnedResult.Items.First().LastName);

            _mockUserService.Verify(s => s.GetPaginatedUsersAsync(
                0, 10, searchTerm, null, "lastName", "asc"), Times.Once);
        }
    }
}