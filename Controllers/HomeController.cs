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
    public class HomeController : Controller
    {
        private UserManager<AppUser> userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var data = GetData(nameof(Index));
            return View(data);
        }

        //[Authorize(Roles = "Users")]
        [Authorize(Policy = "DCStatePolicy")]
        public IActionResult OtherAction()
        {
            return View("Index", GetData(nameof(OtherAction)));
        }

        [Authorize(Policy = "BlockSaad")]
        public IActionResult BlockSaadAction()
        {
            return View("Index", GetData(nameof(BlockSaadAction)));
        }

        private Dictionary<string, object> GetData(string actionName)
        {
            return new Dictionary<string, object>
            {
                ["Action"] = actionName,
                ["User"] = HttpContext.User.Identity.Name,
                ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
                ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
                ["In Users Role"] = HttpContext.User.IsInRole("Users"),
                ["City"] = CurrentUser.Result.City,
                ["Qualification"] = CurrentUser.Result.Qualification
            };
        }

        [Authorize]
        public async Task<IActionResult> UserProps() => View(await CurrentUser);
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserProps([Required] Cities city, [Required] QualificationLevels qualification)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await CurrentUser;
                user.City = city; 
                user.Qualification = qualification;
                await userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return View(await CurrentUser);
        }

        private Task<AppUser> CurrentUser => userManager.FindByNameAsync(HttpContext.User.Identity.Name);
    }
}