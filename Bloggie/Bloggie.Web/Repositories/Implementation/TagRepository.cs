using Azure;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories.Implementation
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext context;

        public TagRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Tag> AddAsync(Tag tag)
        {
            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();
            return tag;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync(
            string? searchQuery, 
            string? sortBy, 
            string? sortDirection,
            int pageSize = 1,
            int pageNumber = 100)
        {
            var query = context.Tags.AsQueryable();

            // Filtering
            if (string.IsNullOrWhiteSpace(searchQuery) == false)
            {
                query = query.Where(x => x.Name.Contains(searchQuery) ||
                                         x.DisplayName.Contains(searchQuery));
            }

            // Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                var isDesc = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase);

                if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                }

                if(string.Equals(sortBy, "DisplayName", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.DisplayName) : query.OrderBy(x => x.DisplayName);
                }

            }

            // Pagination
            // Skip 0 Take 5 -> Page 1 of 5 Results
            var skipResults = (pageNumber - 1) * pageSize;
            query = query.Skip(skipResults).Take(pageSize);

            // Return result
            return await query.ToListAsync();
            //return await context.Tags.ToListAsync();
        }

        public Task<Tag?> GetAsync(Guid id)
        {
           return context.Tags.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
           var existingTag = await context.Tags.FindAsync(tag.Id);

            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                await context.SaveChangesAsync();

                return existingTag;
            }

            return null;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var existingTag = await context.Tags.FindAsync(id);

            if (existingTag != null)
            {
                context.Tags.Remove(existingTag);
                await context.SaveChangesAsync();
                return existingTag;
            }

            return null;
        }

        public async Task<int> CountAsync()
        {
           return await context.Tags.CountAsync();
        }

       
    }
}
