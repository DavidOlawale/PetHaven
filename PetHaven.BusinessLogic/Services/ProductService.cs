using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IAzureBlobService _blobService;

        public ProductService(IProductRepository productRepository, IAzureBlobService blobService)
        {
            _productRepository = productRepository;
            _blobService = blobService;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(CreateProductDTO productDto)
        {

            var imageUrls = new List<string>();
            foreach (var image in productDto.Images!)
            {
                if (image != null && image.Length > 0)
                {
                    var imageUrl = await _blobService.UploadImageAsync(image);
                    imageUrls.Add(imageUrl);
                }
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                DiscountedPrice = productDto.DiscountedPrice,
                OriginalPrice = productDto.OriginalPrice,
                Category = productDto.Category,
                Stock = productDto.Stock,
                Brand = productDto.Brand,
                Weight = productDto.Weight,
                AnimalType = productDto.AnimalType,
                ImageUrls = string.Join(",", imageUrls)
            };

            return await _productRepository.CreateProductAsync(product);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            return await _productRepository.UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _productRepository.GetProductsByCategoryAsync(category);
        }
    }
}
