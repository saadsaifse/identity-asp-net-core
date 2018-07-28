using Microsoft.AspNetCore.Identity;

namespace AspIdentity.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public Cities City { get; set; }
        public QualificationLevels Qualification { get; set; }
    }
}