using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Models.ViewModels
{
    public class AddBlogPostRequest
    {
        public string Heading { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string UrlHandle { get; set; }

        public DateTime PublishedDate { get; set; }

        public string Author { get; set; }

        public bool Visible { get; set; }

        // Dispaly Tags
        public IEnumerable<SelectListItem> Tags { get; set; }

        // Collect tag
        public string[] SelectedTags { get; set; } = Array.Empty<string>();
    }
}
