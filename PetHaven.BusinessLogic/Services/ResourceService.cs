using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IResourceRepository _resourceRepository;

        public ResourceService(IResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            resource.PublishedDate = DateTime.UtcNow;
            return await _resourceRepository.AddResourceAsync(resource);
        }

        public async Task DeleteResourceAsync(int id)
        {
            await _resourceRepository.DeleteResourceAsync(id);
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            return await _resourceRepository.GetAllResourcesAsync();
        }

        public async Task<Resource?> GetResourceByIdAsync(int id)
        {
            return await _resourceRepository.GetResourceByIdAsync(id);
        }

        public async Task<Resource> UpdateResourceAsync(Resource resource)
        {
            return await _resourceRepository.UpdateResourceAsync(resource);
        }
    }
}