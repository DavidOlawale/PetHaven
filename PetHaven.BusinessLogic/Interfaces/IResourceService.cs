using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Interfaces
{
    public interface IResourceService
    {
        Task<Resource?> GetResourceByIdAsync(int id);
        Task<IEnumerable<Resource>> GetAllResourcesAsync();
        Task<Resource> CreateResourceAsync(Resource resource);
        Task<Resource> UpdateResourceAsync(Resource resource);
        Task DeleteResourceAsync(int id);
    }
}
