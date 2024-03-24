using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bloggie.Web.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext context;

        public BlogPostRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await context.AddAsync(blogPost);
            await context.SaveChangesAsync();
            return blogPost;    
                
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlogPost = await context.BlogPosts.FindAsync(id);

            if (existingBlogPost != null)
            {
                context.BlogPosts.Remove(existingBlogPost);
                await context.SaveChangesAsync();
                return existingBlogPost;
            }
            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
           return await context.BlogPosts.Include(x => x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
           return await context.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id); 
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlog = await context.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id ==  blogPost.Id);

            if (existingBlog != null)
            {
                existingBlog.Id = blogPost.Id;
                existingBlog.Heading = blogPost.Heading;
                existingBlog.Title = blogPost.Title;
                existingBlog.Content = blogPost.Content;
                existingBlog.Description = blogPost.Description;
                existingBlog.ImageUrl = blogPost.ImageUrl;
                existingBlog.UrlHandle = blogPost.UrlHandle;
                existingBlog.PublishedDate = blogPost.PublishedDate;
                existingBlog.Author = blogPost.Author;
                existingBlog.Visible = blogPost.Visible;
                existingBlog.Tags = blogPost.Tags;

                await context.SaveChangesAsync();
                return existingBlog;
            }

            return null;
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await context.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);




           
        }
    }
}
