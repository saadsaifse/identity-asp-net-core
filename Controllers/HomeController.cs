using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace AspIdentity.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(){
            return View(new Dictionary<string, object>{
                ["Admin Link"] = "/admin"
             });
        }
    }
}