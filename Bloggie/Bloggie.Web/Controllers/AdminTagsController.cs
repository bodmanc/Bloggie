using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AdminTagsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public IActionResult Add(AddTagRequest addTagRequest)
        {
            // mapping AddTagRequest to domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            dbContext.Tags.Add(tag);
            dbContext.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public IActionResult List()
        {
            // Use db context to read tags
            var tags = dbContext.Tags.ToList();
            return View(tags);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var tag = dbContext.Tags.FirstOrDefault(x => x.Id == id);
            
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
        public IActionResult Edit(EditTagRequest editTagRequest)
        {
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };
            var existingTag = dbContext.Tags.Find(tag.Id);

            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                // save changes 
                dbContext.SaveChanges();
                // Show success notification

                return RedirectToAction("List", new { id = editTagRequest.Id });
            }
            // Show failure notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        [HttpPost]
        public IActionResult Delete(EditTagRequest editTagRequest)
        {
            var tag = dbContext.Tags.Find(editTagRequest.Id);

            if(tag != null)
            {
                dbContext.Tags.Remove(tag);
                dbContext.SaveChanges();

                // show success notificaiton
                return RedirectToAction("List");
            }

            // show error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }
    }
}
