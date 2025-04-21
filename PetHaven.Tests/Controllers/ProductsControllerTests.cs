using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductsController _controller;
        private readonly int _testUserId = 5;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductsController(_mockProductService.Object);

            // Simulate authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()),
                new Claim(ClaimTypes.Role, nameof(UserRoles.Administrator)) // Add admin role for authorized endpoints
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
        public async Task GetProducts_ReturnsOkWithAllProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Dog Food Premium",
                    Description = "High quality dog food",
                    OriginalPrice = 29,
                    Category = "Food",
                    Stock = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Cat Toy Set",
                    Description = "Set of 5 cat toys",
                    OriginalPrice = 15,
                    DiscountedPrice = 12.5m,
                    Category = "Toys",
                    Stock = 50
                }
            };

            _mockProductService.Setup(s => s.GetAllProductsAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(expectedProducts, returnedProducts);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkWithProduct_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            var expectedProduct = new Product
            {
                Id = productId,
                Name = "Dog Food Premium",
                Description = "High quality dog food",
                OriginalPrice = 29.99m,
                Category = "Food",
                Stock = 100
            };

            _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(expectedProduct, returnedProduct);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;

            _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtActionWithProduct_WhenAuthorized()
        {
            // Arrange
            var productDto = new CreateProductDTO
            {
                Name = "New Dog Food",
                Description = "New premium dog food",
                OriginalPrice = 39.99m,
                Category = "Food",
                Stock = 75,
                Brand = "PetCare",
                AnimalType = new List<string> { "Dog" }
            };

            var createdProduct = new Product
            {
                Id = 3,
                Name = productDto.Name,
                Description = productDto.Description,
                OriginalPrice = productDto.OriginalPrice,
                Category = productDto.Category,
                Stock = productDto.Stock,
                Brand = productDto.Brand,
                AnimalType = productDto.AnimalType
            };

            _mockProductService.Setup(s => s.CreateProductAsync(productDto))
                .ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.CreateProduct(productDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(ProductsController.GetProduct), createdAtActionResult.ActionName);
            Assert.Equal(createdProduct.Id, createdAtActionResult.RouteValues["id"]);

            var returnedProduct = Assert.IsType<Product>(createdAtActionResult.Value);
            Assert.Equal(createdProduct, returnedProduct);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_WhenProductExistsAndAuthorized()
        {
            // Arrange
            int productId = 1;
            var updateDto = new UpdateProductDto
            {
                Id = productId,
                Name = "Updated Dog Food",
                Description = "Updated premium dog food",
                OriginalPrice = 35.99m,
                DiscountedPrice = 29.99m,
                Category = "Food",
                Stock = 150,
                Brand = "PetCare Plus",
                ExistingImageUrls = "image1.jpg,image2.jpg"
            };

            var updatedProduct = new Product
            {
                Id = productId,
                Name = updateDto.Name,
                Description = updateDto.Description,
                OriginalPrice = updateDto.OriginalPrice,
                DiscountedPrice = updateDto.DiscountedPrice,
                Category = updateDto.Category,
                Stock = updateDto.Stock,
                Brand = updateDto.Brand,
                ImageUrls = updateDto.ExistingImageUrls
            };

            _mockProductService.Setup(s => s.UpdateProductAsync(updateDto))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.UpdateProduct(productId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            int productId = 1;
            var updateDto = new UpdateProductDto
            {
                Id = 2, // Different from route parameter
                Name = "Updated Dog Food",
                Description = "Updated premium dog food",
                OriginalPrice = 35.99m,
                DiscountedPrice = 29.99m,
                Category = "Food",
                Stock = 150
            };

            // Act
            var result = await _controller.UpdateProduct(productId, updateDto);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
            _mockProductService.Verify(s => s.UpdateProductAsync(It.IsAny<UpdateProductDto>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;
            var updateDto = new UpdateProductDto
            {
                Id = productId,
                Name = "Updated Dog Food",
                Description = "Updated premium dog food",
                OriginalPrice = 35.99m,
                DiscountedPrice = 29.99m,
                Category = "Food",
                Stock = 150
            };

            _mockProductService.Setup(s => s.UpdateProductAsync(updateDto))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.UpdateProduct(productId, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenProductExistsAndAuthorized()
        {
            // Arrange
            int productId = 1;

            _mockProductService.Setup(s => s.DeleteProductAsync(productId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;

            _mockProductService.Setup(s => s.DeleteProductAsync(productId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProductsByCategory_ReturnsOkWithCategoryProducts()
        {
            // Arrange
            string category = "Food";
            var expectedProducts = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Dog Food Premium",
                    Description = "High quality dog food",
                    OriginalPrice = 29.99m,
                    Category = category,
                    Stock = 100
                },
                new Product
                {
                    Id = 3,
                    Name = "Cat Food Premium",
                    Description = "High quality cat food",
                    OriginalPrice = 27.99m,
                    Category = category,
                    Stock = 85
                }
            };

            _mockProductService.Setup(s => s.GetProductsByCategoryAsync(category))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetProductsByCategory(category);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(expectedProducts, returnedProducts);
        }
    }
}