using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IBlogPostRepository blogPostRepository;

        public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Get tags from repository
            var tags = await tagRepository.GetAllAsync();

            // create new view model
            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.DisplayName, Value = x.Id.ToString() })
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            // Map view model to domain model
            var blogPostDomain = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                Title = addBlogPostRequest.Title,
                Content = addBlogPostRequest.Content,
                Description = addBlogPostRequest.Description,
                ImageUrl = addBlogPostRequest.ImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,
            };

            // Map tags from selected tags
            var selectedTags = new List<Tag>();

            foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);

                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);

                }
            }

            // Mapping tags back to domain model
            blogPostDomain.Tags = selectedTags;

            await blogPostRepository.AddAsync(blogPostDomain);
            return RedirectToAction("List");
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            // Call Repository
            var blogPosts = await blogPostRepository.GetAllAsync();
            return View(blogPosts);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Retrieve the result from the repository
            var blogPost = await blogPostRepository.GetAsync(id);
            var tagsDomainModel = await tagRepository.GetAllAsync();

            if (blogPost != null)
            {
                // Map the domain model into the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    Title = blogPost.Title,
                    Content = blogPost.Content,
                    Description = blogPost.Description,
                    ImageUrl = blogPost.ImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    Visible = blogPost.Visible,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList(),

                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };
                // Pass data to view
                return View(model);
            }

            return View(null);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
          
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                Title = editBlogPostRequest.Title,
                Content = editBlogPostRequest.Content,
                Description = editBlogPostRequest.Description,
                ImageUrl = editBlogPostRequest.ImageUrl,
                UrlHandle = editBlogPostRequest.UrlHandle,
                PublishedDate = editBlogPostRequest.PublishedDate,
                Author = editBlogPostRequest.Author,
                Visible = editBlogPostRequest.Visible
            };

            var selectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if (Guid.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            var updatedBlogPost = await blogPostRepository.UpdateAsync(blogPostDomainModel);

            if (updatedBlogPost != null)
            {
                // Update successful, redirect to appropriate action
                return RedirectToAction("List");
            }

            // Update failed, return to edit view with error message
            ModelState.AddModelError("", "Failed to update blog post.");
            return View(editBlogPostRequest);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            // Talk to repository to delete this blog post
            var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedBlogPost != null)
            {
                // Show success response
                return RedirectToAction("List");
            }

            // Show error response 
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }


    }
}