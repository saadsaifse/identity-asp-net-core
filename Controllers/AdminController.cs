using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspIdentity.Models;
using AspIdentity.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspIdentity.Controllers
{
    [Authorize(Roles ="Admins")]
    public class AdminController : Controller
    {
        readonly UserManager<AppUser> userManager;
        readonly IPasswordHasher<AppUser> passwordHasher;
        readonly IUserValidator<AppUser> userValidator;
        readonly IPasswordValidator<AppUser> passwordValidator;


        public AdminController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> hasher, IUserValidator<AppUser> userValidator, IPasswordValidator<AppUser> passValidator)
        {
            this.userManager = userManager;
            this.passwordHasher = hasher;
            this.userValidator = userValidator;
            this.passwordValidator = passValidator;
        }

        public ViewResult Index()
        {
            var users = userManager.Users;
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);

            ModelState.AddModelError("", "User not found!");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string email, string password)
        {
            if (id == null || password == null || email == null)
                ModelState.AddModelError("", "All fields are required");
            else
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null)
                    ModelState.AddModelError("", "User not found!");
                else
                {
                    user.Email = email;
                    var validateEmailTask = await userValidator.ValidateAsync(userManager, user);

                    if (!validateEmailTask.Succeeded)
                        AddIdentityErrors(validateEmailTask.Errors);
                    else
                    {
                        var validatePasswordTask = await passwordValidator.ValidateAsync(userManager, user, password);
                        if (!validatePasswordTask.Succeeded)
                            AddIdentityErrors(validatePasswordTask.Errors);
                        else
                        {
                            var passwordHash = passwordHasher.HashPassword(user, password);
                            user.PasswordHash = passwordHash;

                            var updateTask = await userManager.UpdateAsync(user);
                            if (!updateTask.Succeeded)
                                AddIdentityErrors(updateTask.Errors);
                            else
                                return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            return View();
        }

        private void AddIdentityErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        public ViewResult Create()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Name, Email = model.Email };
                var validatePasswordTask = await passwordValidator.ValidateAsync(userManager, user, model.Password);
                if (!validatePasswordTask.Succeeded)
                    AddIdentityErrors(validatePasswordTask.Errors);
                else
                {
                    var passwordHash = passwordHasher.HashPassword(user, model.Password);
                    var result = await userManager.CreateAsync(new AppUser { UserName = model.Name, Email = model.Email, PasswordHash = passwordHash });
                    if (!result.Succeeded)
                        AddIdentityErrors(result.Errors);
                    else
                        return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(usr => usr.Id == id);

            if (user != null)
            {
                var task = await userManager.DeleteAsync(user);
                if (!task.Succeeded)
                    AddIdentityErrors(task.Errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}