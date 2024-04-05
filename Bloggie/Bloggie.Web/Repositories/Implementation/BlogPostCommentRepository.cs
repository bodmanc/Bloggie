using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories.Implementation
{
    public class BlogPostCommentRepository : IBlogPostCommentRepository
    {
        private readonly ApplicationDbContext context;

        public BlogPostCommentRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
        {
            await context.BlogPostComments.AddAsync(blogPostComment);
            await context.SaveChangesAsync();   
            return blogPostComment;
        }

        public async Task<IEnumerable<BlogPostComment>> GetCommentsByBlogIdAsync(Guid blogPostId)
        {
            return await context.BlogPostComments.Where(x => x.BlogPostId == blogPostId).ToListAsync();
        }
    }
}
