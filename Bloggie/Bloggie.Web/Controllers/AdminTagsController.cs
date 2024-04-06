using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            ValidateAddTagRequest(addTagRequest);
            if (ModelState.IsValid)
            {
                // mapping AddTagRequest to domain model
                var tag = new Tag
                {
                    Name = addTagRequest.Name,
                    DisplayName = addTagRequest.DisplayName
                };
                await tagRepository.AddAsync(tag);
                return RedirectToAction("List");
            }

            return View(addTagRequest);
           
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List(
            string? searchQuery, 
            string? sortBy, 
            string? sortDirection,
            int pageSize = 3,
            int pageNumber = 1)
        {
            var totalRecords = await tagRepository.CountAsync();
            var totalPages = Math.Ceiling((decimal)totalRecords / pageSize);


            if (pageNumber > totalPages)
            {
                pageNumber--;
            }

            if (pageNumber < 1)
            {
                pageNumber++;
            }


            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            // Use db context to read tags
            var tags = await tagRepository.GetAllAsync(searchQuery, sortBy, sortDirection, pageSize, pageNumber);
          
            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
           var tag = await tagRepository.GetAsync(id);
            
            if (tag != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };

                return View(editTagRequest);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };
            var updatedTag =  await tagRepository.UpdateAsync(tag);

            if (updatedTag != null)
            {
                // show success notification
                return RedirectToAction("List");
            }
            else
            {
                // Show failure notification
            }
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
          
            var deletedTag = await tagRepository.DeleteAsync(editTagRequest.Id);

            if (deletedTag != null)
            {
                // show success notification
                return RedirectToAction("List");
            }
            else
            {
                // show error notification
                return RedirectToAction("Edit", new { id = editTagRequest.Id });
            }
           
        }

        private void ValidateAddTagRequest(AddTagRequest addTagRequest)
        {
            if (addTagRequest.Name != null && addTagRequest.DisplayName != null)
            {
                if (addTagRequest.Name == addTagRequest.DisplayName)
                {
                    ModelState.AddModelError("DisplayName", "Name cannot be same as display name");
                }
            }
        }

    }
}
