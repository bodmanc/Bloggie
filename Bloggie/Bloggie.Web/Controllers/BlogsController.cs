using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories.Interface;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly IBlogPostLikeRepository blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;

        public BlogsController(IBlogPostRepository blogPostRepository,
            IBlogPostLikeRepository blogPostLikeRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IBlogPostCommentRepository blogPostCommentRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.blogPostLikeRepository = blogPostLikeRepository;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.blogPostCommentRepository = blogPostCommentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var liked = false;
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);

            if (blogPost != null)
            {
                var totalLikes = await blogPostLikeRepository.GetTotalLikes(blogPost.Id);

                if (signInManager.IsSignedIn(User))
                {
                    // Get likes for this blog for this user
                    var likesForBlog = await blogPostLikeRepository.GetLikesForBlog(blogPost.Id);

                    var userId = userManager.GetUserId(User);

                    if (userId != null)
                    {
                      var likeFromUser =  likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                      liked = likeFromUser != null;
                    }

                }

                // Get comments for blog post
                var blogCommentsDomainModel = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPost.Id);

                var blogCommentsForView = new List<BlogComment>();
                
                foreach (var blogComment in blogCommentsDomainModel)
                {
                    blogCommentsForView.Add(new BlogComment
                    {
                        Description = blogComment.Description,
                        DateAdded = blogComment.DateAdded,
                        UserName = (await userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName
                    });
                }

                var blogDetailsViewModel = new BlogDetailsViewModel
                {
                    Id = blogPost.Id,
                    Content = blogPost.Content,
                    Title = blogPost.Title,
                    Author = blogPost.Author,
                    ImageUrl = blogPost.ImageUrl,
                    Heading = blogPost.Heading,
                    PublishedDate = blogPost.PublishedDate,
                    Description = blogPost.Description,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    Liked = liked,
                    Comments = blogCommentsForView
                };

                return View(blogDetailsViewModel);
            }

               return NotFound(); 
        }

        [HttpPost]

        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
            if (signInManager.IsSignedIn(User))
            {
                var domainModel = new BlogPostComment
                {
                    BlogPostId = blogDetailsViewModel.Id,
                    Description = blogDetailsViewModel.CommentDescription,
                    UserId = Guid.Parse(userManager.GetUserId(User)),
                    DateAdded = DateTime.Now
                };

                await blogPostCommentRepository.AddAsync(domainModel);
                return RedirectToAction("Index", "Home", 
                    new {urlHandle = blogDetailsViewModel.UrlHandle});
                
            }

            return Forbid();
        }
    }
}
