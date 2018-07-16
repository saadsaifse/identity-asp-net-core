using System.ComponentModel.DataAnnotations;

namespace AspIdentity.Models
{
    public class UserViewModel
    {
        [Required]
        public string Name { get; set; }
       
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [UIHint("Password")]
        public string Password { get; set; }
    }
}