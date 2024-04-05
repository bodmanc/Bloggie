using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Models.ViewModels
{
    public class BlogDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Heading { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string UrlHandle { get; set; }

        public DateTime PublishedDate { get; set; }

        public string Author { get; set; }

        public bool Visible { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public int TotalLikes { get; set; }

        public bool Liked { get; set; }

        public string CommentDescription { get; set; }
        public IEnumerable<BlogComment> Comments { get; set; }

    }
}
