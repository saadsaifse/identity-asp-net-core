using System.ComponentModel.DataAnnotations;

namespace AspIdentity.Models
{
    public class RoleEditViewModel
    {
        public string RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
        public string[] MemberToAddIds { get; set; }
        public string[] MemberToRemoveIds { get; set; }
    }
}