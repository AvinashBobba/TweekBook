using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TweekBook.Authorization
{
    public class WorksForComapnyHandler : AuthorizationHandler<WorksForCompanyRequirements>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WorksForCompanyRequirements requirement)
        {
            var userEmail = context.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            if(userEmail.EndsWith(requirement.DomainName))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
