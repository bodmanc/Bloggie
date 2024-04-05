using Bloggie.Web.Data;
using Bloggie.Web.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext context;

        public UserRepository(AuthDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<IdentityUser>> GetAll()
        {
           var users = await context.Users.ToListAsync();

           var superAdminUser = await context.Users.
                FirstOrDefaultAsync(x => x.Email == "superadmin@bloggie.com");

           if (superAdminUser != null) 
           { 
                users.Remove(superAdminUser);
           } 

           return users;
        }
    }
}
