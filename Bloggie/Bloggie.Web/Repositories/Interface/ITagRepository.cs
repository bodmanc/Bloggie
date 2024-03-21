using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories.Interface
{
    public interface ITagRepository
    {
        // Get all tags
       Task<IEnumerable<Tag>> GetAllAsync();

        // Get tag by id
       Task<Tag?> GetAsync(Guid id);

        // Add tag
       Task<Tag> AddAsync(Tag tag);

        // Update tag
       Task<Tag?> UpdateAsync(Tag tag);

        // Delete tag
       Task<Tag?> DeleteAsync(Guid tag);
    }
}
