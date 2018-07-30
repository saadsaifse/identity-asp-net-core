using System;
using System.Linq;
using System.Threading.Tasks;
using AspIdentity.Models;
using Microsoft.AspNetCore.Authorization;

namespace AspIdentity.Infrastructure
{
    public class ProtectCasesRequirement : IAuthorizationRequirement
    {
        public ProtectCasesRequirement(bool onlyCreator)
        {
            RestrictAccesToCreatorOnly = onlyCreator;
        }
        public bool RestrictAccesToCreatorOnly { get; set; }
    }

    public class ProtectCaseHandler : AuthorizationHandler<ProtectCasesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProtectCasesRequirement requirement)
        {
            var confidentialCase = context.Resource as ConfidentialCase;
            if (confidentialCase != null)
            {
                string user = context.User.Identity.Name;
                if (user.Equals(confidentialCase.CreatedBy, StringComparison.OrdinalIgnoreCase) ||
                    (!requirement.RestrictAccesToCreatorOnly && confidentialCase.UserCanView.Any(u => u.Equals(user, StringComparison.OrdinalIgnoreCase))))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }
    }
}