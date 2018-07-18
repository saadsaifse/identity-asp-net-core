using System.ComponentModel.DataAnnotations;

namespace AspIdentity.Models.Identity
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [UIHint("Password")]
        public string Password { get; set; }
    }
}