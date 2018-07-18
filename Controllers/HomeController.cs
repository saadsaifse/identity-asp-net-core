using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspIdentity.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult Index(){
            return View(new Dictionary<string, object>{
                ["Admin Link"] = "/admin"
             });
        }
    }
}