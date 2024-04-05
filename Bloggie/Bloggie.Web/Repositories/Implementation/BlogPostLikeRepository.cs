using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories.Implementation
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {
        private readonly ApplicationDbContext context;

        public BlogPostLikeRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike)
        {
            await context.BlogPostLikes.AddAsync(blogPostLike);
            await context.SaveChangesAsync();
            return blogPostLike;
        }

        public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
        {
            return await context.BlogPostLikes.Where(x => x.BlogPostId == blogPostId).ToListAsync();
        }

        public async Task<int> GetTotalLikes(Guid blogPostId)
        {
           return await context.BlogPostLikes.CountAsync(x => x.BlogPostId == blogPostId);
        }

        
    }
}
