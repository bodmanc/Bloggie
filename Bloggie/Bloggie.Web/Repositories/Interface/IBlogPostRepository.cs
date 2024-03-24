using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories.Interface
{
    public interface IBlogPostRepository
    {
        // Get all tags
        Task<IEnumerable<BlogPost>> GetAllAsync();

        // Get tag by id
        Task<BlogPost?> GetAsync(Guid id);

        // Add tag
        Task<BlogPost> AddAsync(BlogPost blogPost);

        // Update tag
        Task<BlogPost?> UpdateAsync(BlogPost blogPost);

        // Delete tag
        Task<BlogPost?> DeleteAsync(Guid id);

        // 
        Task<BlogPost?> GetByUrlHandleAsync(string urlHandle);
    }
}
