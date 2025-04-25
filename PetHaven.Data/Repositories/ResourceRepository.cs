using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly AppDbContext _context;

        public ResourceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Resource?> GetResourceByIdAsync(int id)
        {
            return await _context.Resources.FindAsync(id);
        }

        public async Task<List<Resource>> GetAllResourcesAsync(string? category = null)
        {
            var query = _context.Resources.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(r => r.Category == category);
            }

            return await query.ToListAsync();
        }

        public async Task<Resource> AddResourceAsync(Resource resource)
        {
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<Resource> UpdateResourceAsync(Resource resource)
        {
            var dbResource = await GetResourceByIdAsync(resource.Id);
            dbResource.Title = resource.Title;
            dbResource.Content = resource.Content;
            dbResource.Category = resource.Category;
            dbResource.ImageUrl = resource.ImageUrl;
            _context.Resources.Update(dbResource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task DeleteResourceAsync(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource != null)
            {
                _context.Resources.Remove(resource);
                await _context.SaveChangesAsync();
            }
        }
    }
}