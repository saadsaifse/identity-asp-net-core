using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AspIdentity.Models;
using AspIdentity.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspIdentity.Controllers
{
    [Authorize(Roles ="Admins")]
    public class RoleAdminController : Controller
    {
        readonly RoleManager<IdentityRole> roleManager;
        readonly UserManager<AppUser> userManager;
        public RoleAdminController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(roleManager.Roles);
        }
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            List<AppUser> nonMembers = new List<AppUser>();
            List<AppUser> members = new List<AppUser>();

            if (role == null)
                return NotFound();
            else
            {
                foreach (var user in userManager.Users)
                {
                    bool belongs = await userManager.IsInRoleAsync(user, role.Name);
                    if (belongs) members.Add(user);
                    else nonMembers.Add(user);
                }
                return View(new RoleViewModel
                {
                    Role = role,
                    Members = members,
                    NonMembers = nonMembers
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleEditViewModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.MemberToAddIds ?? new string[] { })
                {
                    AppUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddIdentityErrors(result.Errors);
                        }
                    }
                }
                foreach (string userId in model.MemberToRemoveIds ?? new string[] { })
                {
                    AppUser user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddIdentityErrors(result.Errors);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return await Edit(model.RoleId);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {

                var role = new IdentityRole(name);
                var roleTask = await roleManager.CreateAsync(role);

                if (roleTask.Succeeded)
                    return RedirectToAction(nameof(Index));
                else
                    AddIdentityErrors(roleTask.Errors);
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult deleteTask = await roleManager.DeleteAsync(role);
                if (deleteTask.Succeeded)
                    return RedirectToAction(nameof(Index));
                else
                    AddIdentityErrors(deleteTask.Errors);
            }
            else
            {
                ModelState.AddModelError("", "No role found");
            }
            return View("Index", roleManager.Roles);
        }

        private void AddIdentityErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}