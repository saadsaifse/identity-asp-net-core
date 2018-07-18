using System.Collections.Generic;
using AspIdentity.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace AspIdentity.Models
{
    public class RoleViewModel
    {
        public IdentityRole Role  { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }
}