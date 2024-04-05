using Microsoft.AspNetCore.Identity;

namespace Bloggie.Web.Repositories.Interface
{
    public interface IUserRepository
    {
       Task<IEnumerable<IdentityUser>> GetAll();
    }
}
