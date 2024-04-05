using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories.Interface
{
    public interface IBlogPostLikeRepository
    {
       Task<int> GetTotalLikes(Guid blogPostId);

       Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId);

       Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike);
    }
}
