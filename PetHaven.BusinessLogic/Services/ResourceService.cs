using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly IUserRepository _userRepository;

        public ResourceService(IResourceRepository resourceRepository, IUserRepository userRepository)
        {
            _resourceRepository = resourceRepository;
            _userRepository = userRepository;
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            resource.PublishedDate = DateTime.UtcNow;
            var author = await _userRepository.GetUserByIdAsync(resource.CreatorId);
            resource.Author = $"{author.FirstName} {author.LastName}";
            return await _resourceRepository.AddResourceAsync(resource);
        }

        public async Task DeleteResourceAsync(int id)
        {
            await _resourceRepository.DeleteResourceAsync(id);
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync(string? category = null)
        {
            return await _resourceRepository.GetAllResourcesAsync(category);
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