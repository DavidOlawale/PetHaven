using PetHaven.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories.Interfaces
{
    public interface IResourceRepository
    {
        Task<Resource?> GetResourceByIdAsync(int id);
        Task<List<Resource>> GetAllResourcesAsync();
        Task<Resource> AddResourceAsync(Resource resource);
        Task UpdateResourceAsync(Resource resource);
        Task DeleteResourceAsync(int id);
    }   
}