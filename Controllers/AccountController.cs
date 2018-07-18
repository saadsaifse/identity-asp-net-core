using System.Threading.Tasks;
using AspIdentity.Models;
using AspIdentity.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspIdentity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        readonly UserManager<AppUser> userManager;
        readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnURL)
        {
            ViewBag.ReturnURL = returnURL;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                await signInManager.SignOutAsync();
                var singInTask = await signInManager.PasswordSignInAsync(details.Username, details.Password,false, false);
                if (singInTask.Succeeded)
                    return Redirect(returnUrl ?? "/");
                
                ModelState.AddModelError("", "Incorrect Username or Password");
            }

            return View(details);
        }
    }
}