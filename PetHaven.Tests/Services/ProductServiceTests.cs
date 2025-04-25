using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using Xunit;

namespace PetHaven.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IBlobService> _mockBlobService;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockBlobService = new Mock<IBlobService>();
            _service = new ProductService(_mockProductRepository.Object, _mockBlobService.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Dog Food Premium",
                    Description = "High quality dog food",
                    OriginalPrice = 29.99m,
                    Category = "Food",
                    Stock = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Cat Toy Set",
                    Description = "Set of 5 cat toys",
                    OriginalPrice = 15.99m,
                    DiscountedPrice = 12.99m,
                    Category = "Toys",
                    Stock = 50
                }
            };

            _mockProductRepository.Setup(r => r.GetAllProductsAsync())
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Equal(expectedProducts, result);
            _mockProductRepository.Verify(r => r.GetAllProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProduct_WhenProductExists()
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

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _service.GetProductByIdAsync(productId);

            // Assert
            Assert.Equal(expectedProduct, result);
            _mockProductRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsNull_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 999;

            _mockProductRepository.Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _service.GetProductByIdAsync(productId);

            // Assert
            Assert.Null(result);
            _mockProductRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_CreatesProduct_WithImages()
        {
            // Arrange
            var imageList = new List<string>
            {
                "data:image/jpeg;base64,/9j/4AAQSkZJRgAB...",
                "data:image/jpeg;base64,/9j/4AAQSkZJRgAB..."
            };

            var productDto = new CreateProductDTO
            {
                Name = "New Dog Food",
                Description = "New premium dog food",
                OriginalPrice = 39.99m,
                Category = "Food",
                Stock = 75,
                Brand = "PetCare",
                AnimalType = new List<string> { "Dog" },
                Images = imageList
            };

            var imageUrls = new List<string>
            {
                "https://storage.blob.core.windows.net/products/image1.jpg",
                "https://storage.blob.core.windows.net/products/image2.jpg"
            };

            for (int i = 0; i < imageList.Count; i++)
            {
                _mockBlobService.Setup(b => b.UploadImageAsync(imageList[i], null))
                    .ReturnsAsync(imageUrls[i]);
            }

            Product capturedProduct = null;
            var createdProduct = new Product
            {
                Id = 3,
                Name = productDto.Name,
                Description = productDto.Description,
                OriginalPrice = productDto.OriginalPrice,
                Category = productDto.Category,
                Stock = productDto.Stock,
                Brand = productDto.Brand,
                AnimalType = productDto.AnimalType,
                ImageUrls = string.Join(",", imageUrls)
            };

            _mockProductRepository.Setup(r => r.CreateProductAsync(It.IsAny<Product>()))
                .Callback<Product>(p => capturedProduct = p)
                .ReturnsAsync(createdProduct);

            // Act
            var result = await _service.CreateProductAsync(productDto);

            // Assert
            Assert.Equal(createdProduct, result);

            // Verify product was created with correct properties
            Assert.NotNull(capturedProduct);
            Assert.Equal(productDto.Name, capturedProduct.Name);
            Assert.Equal(productDto.Description, capturedProduct.Description);
            Assert.Equal(productDto.OriginalPrice, capturedProduct.OriginalPrice);
            Assert.Equal(productDto.Category, capturedProduct.Category);
            Assert.Equal(productDto.Stock, capturedProduct.Stock);
            Assert.Equal(productDto.Brand, capturedProduct.Brand);
            Assert.Equal(productDto.AnimalType, capturedProduct.AnimalType);

            // Verify image URLs were stored correctly
            //Assert.Equal(string.Join(",", imageUrls), capturedProduct.ImageUrls);

            // Verify blob service was called for each image
            foreach (var image in imageList)
            {
                _mockBlobService.Verify(b => b.UploadImageAsync(image, null));
            }

            _mockProductRepository.Verify(r => r.CreateProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_UpdatesProduct_WithNewImages()
        {
            // Arrange
            string existingImageUrls = "https://storage.blob.core.windows.net/products/existing1.jpg,https://storage.blob.core.windows.net/products/existing2.jpg";
            var newImageList = new List<string>
            {
                "data:image/jpeg;base64,/9j/4AAQSkZJRgAB...",
                "data:image/jpeg;base64,/9j/4AAQSkZJRgAB..."
            };

            var updateDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Updated Dog Food",
                Description = "Updated premium dog food",
                OriginalPrice = 45.99m,
                DiscountedPrice = 39.99m,
                Category = "Food",
                Stock = 60,
                Brand = "PetCare Plus",
                ExistingImageUrls = existingImageUrls,
                NewImages = newImageList
            };

            var newImageUrls = new List<string>
            {
                "https://storage.blob.core.windows.net/products/new1.jpg",
                "https://storage.blob.core.windows.net/products/new2.jpg"
            };

            for (int i = 0; i < newImageList.Count; i++)
            {
                _mockBlobService.Setup(b => b.UploadImageAsync(newImageList[i], null))
                    .ReturnsAsync(newImageUrls[i]);
            }

            Product capturedProduct = null;
            var updatedProduct = new Product
            {
                Id = 1,
                Name = updateDto.Name,
                Description = updateDto.Description,
                OriginalPrice = updateDto.OriginalPrice,
                DiscountedPrice = updateDto.DiscountedPrice,
                Category = updateDto.Category,
                Stock = updateDto.Stock,
                Brand = updateDto.Brand,
                // Combined existing and new image URLs
                ImageUrls = existingImageUrls + "," + string.Join(",", newImageUrls)
            };

            _mockProductRepository.Setup(r => r.UpdateProductAsync(It.IsAny<Product>()))
                .Callback<Product>(p => capturedProduct = p)
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _service.UpdateProductAsync(updateDto);

            // Assert
            Assert.Equal(updatedProduct, result);

            // Verify product was updated with correct properties
            Assert.NotNull(capturedProduct);
            Assert.Equal(updateDto.Name, capturedProduct.Name);
            Assert.Equal(updateDto.Description, capturedProduct.Description);
            Assert.Equal(updateDto.OriginalPrice, capturedProduct.OriginalPrice);
            Assert.Equal(updateDto.DiscountedPrice, capturedProduct.DiscountedPrice);
            Assert.Equal(updateDto.Category, capturedProduct.Category);
            Assert.Equal(updateDto.Stock, capturedProduct.Stock);
            Assert.Equal(updateDto.Brand, capturedProduct.Brand);

            //// Verify image URLs were combined correctly
            //var expectedCombinedUrls = existingImageUrls.Split(',')
            //    .Concat(newImageUrls)
            //    .ToList();
            //var actualCombinedUrls = capturedProduct.ImageUrls.Split(',').ToList();

            //Assert.Equal(expectedCombinedUrls.Count, actualCombinedUrls.Count);
            //foreach (var url in expectedCombinedUrls)
            //{
            //    Assert.Contains(url, actualCombinedUrls);
            //}

            // Verify blob service was called for each new image
            foreach (var image in newImageList)
            {
                _mockBlobService.Verify(b => b.UploadImageAsync(image, null));
            }

            _mockProductRepository.Verify(r => r.UpdateProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_DeletesProduct()
        {
            // Arrange
            int productId = 1;
            bool deleteResult = true;

            _mockProductRepository.Setup(r => r.DeleteProductAsync(productId))
                .ReturnsAsync(deleteResult);

            // Act
            var result = await _service.DeleteProductAsync(productId);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(r => r.DeleteProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsProductsInCategory()
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

            _mockProductRepository.Setup(r => r.GetProductsByCategoryAsync(category))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _service.GetProductsByCategoryAsync(category);

            // Assert
            Assert.Equal(expectedProducts, result);
            _mockProductRepository.Verify(r => r.GetProductsByCategoryAsync(category), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsEmptyList_WhenNoCategoryMatches()
        {
            // Arrange
            string category = "NonexistentCategory";
            var emptyList = new List<Product>();

            _mockProductRepository.Setup(r => r.GetProductsByCategoryAsync(category))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetProductsByCategoryAsync(category);

            // Assert
            Assert.Empty(result);
            _mockProductRepository.Verify(r => r.GetProductsByCategoryAsync(category), Times.Once);
        }
    }
}