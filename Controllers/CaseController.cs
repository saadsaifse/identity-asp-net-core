using System.Collections.Generic;
using System.Threading.Tasks;
using AspIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspIdentity.Controllers
{
    public class CaseController:Controller
    {
        private IAuthorizationService authService;
        List<ConfidentialCase> cases = new List<ConfidentialCase>();
        public CaseController(IAuthorizationService authorizationService)
        {
            authService = authorizationService;

            cases.Add(new ConfidentialCase{ Title = "Murder Case", CreatedBy = "Admin", UserCanView = new[]{"Admin"} });
            cases.Add(new ConfidentialCase{ Title = "Divorce Case", CreatedBy = "Admin", UserCanView = new[]{"Admin", "Saad"} });
        }

        public async Task<IActionResult> Index()
        {
            List<ConfidentialCase> trimmedCases = new List<ConfidentialCase>();
            foreach (var c in cases){
                    var result  = await authService.AuthorizeAsync(User, c, "TrimCases");
                    if (result.Succeeded)
                        trimmedCases.Add(c);
            }

            return View(trimmedCases);
        }
    }
}