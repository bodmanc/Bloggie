﻿using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<IdentityUser> userManager;

        public AdminUsersController(IUserRepository userRepository, UserManager<IdentityUser> userManager)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await userRepository.GetAll();

            
            var usersViewModel = new UserViewModel();
            usersViewModel.Users = new List<User>();

            foreach(var user in users)
            {
                usersViewModel.Users.Add(new User
                {
                    Id = Guid.Parse(user.Id),
                    UserName = user.UserName,
                    Email = user.Email
                });
            }
            
            return View(usersViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> List(UserViewModel userViewModel)
        {
            var identityUser = new IdentityUser
            {
                UserName = userViewModel.UserName,
                Email = userViewModel.Email
            };

            var identityResult = await userManager.CreateAsync(identityUser, userViewModel.Password);

            if (identityResult != null) 
            { 
                if (identityResult.Succeeded)
                {
                    // assign roles to this user
                    var roles = new List<string> { "User" };

                    if (userViewModel.AdminRoleCheckbox)
                    {
                        roles.Add("Admin");
                    }

                   identityResult = await userManager.AddToRolesAsync(identityUser, roles);


                   if (identityResult != null && identityResult.Succeeded) 
                   {
                        return RedirectToAction("List", "AdminUsers"); 
                   }

                }
            }

            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            
            if (user != null)
            {
                var identityResult = await userManager.DeleteAsync(user);

                if (identityResult != null && identityResult.Succeeded)
                {
                    return RedirectToAction("List", "AdminUsers");
                }
            }

            return View();
        }
    }
}
