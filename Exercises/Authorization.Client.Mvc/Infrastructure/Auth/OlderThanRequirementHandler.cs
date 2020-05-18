using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Authorization.Client.Mvc.Infrastructure.Auth
{
    public class OlderThanRequirementHandler : AuthorizationHandler<OlderThanRequirement>
    {
        /// <summary>
        /// Makes a decision if authorization is allowed based on a specific requirement.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OlderThanRequirement requirement)
        {
            var hasClaim = context.User.HasClaim(x => x.Type == ClaimTypes.DateOfBirth);
            if (!hasClaim)
            {
                return Task.CompletedTask;
            }

            var dateOfBirth = context.User.FindFirst(x => x.Type == ClaimTypes.DateOfBirth).Value;
            var date = DateTime.Parse(dateOfBirth, new CultureInfo("ru-RU"));
            var canEnterDiff = DateTime.Now.Year - date.Year;
            if (canEnterDiff >= requirement.Years)
            {
                context.Succeed(requirement);

            }
            return Task.CompletedTask;

        }
    }
}