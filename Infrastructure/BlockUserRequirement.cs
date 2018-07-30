using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AspIdentity.Infrastructure
{
    public class BlockUserRequirement : IAuthorizationRequirement
    {
        public BlockUserRequirement(params string[] usernamesToBlock)
        {
            BlockedUsers = usernamesToBlock;
        }
        public string[] BlockedUsers { get; set; }
    }

    public class BlockUserHandler : AuthorizationHandler<BlockUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BlockUserRequirement requirement)
        {
            if (context.User.Identity != null && context.User.Identity.Name != null && 
                !requirement.BlockedUsers.Any(user => user.Equals(context.User.Identity.Name, StringComparison.OrdinalIgnoreCase))) 
            {
                 context.Succeed(requirement);
            }
            else 
            {
                 context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}