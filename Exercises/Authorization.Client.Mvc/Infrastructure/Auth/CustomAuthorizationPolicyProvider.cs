using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Authorization.Client.Mvc.Infrastructure.Auth
{
    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="T:Microsoft.AspNetCore.Authorization.DefaultAuthorizationPolicyProvider" />.
        /// </summary>
        /// <param name="options">The options used to configure this instance.</param>
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Gets a <see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationPolicy" /> from the given <paramref name="policyName" />
        /// </summary>
        /// <param name="policyName">The policy name to retrieve.</param>
        /// <returns>The named <see cref="T:Microsoft.AspNetCore.Authorization.AuthorizationPolicy" />.</returns>
        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policyExists = await base.GetPolicyAsync(policyName);
            if (policyExists == null)
            {
                policyExists = new AuthorizationPolicyBuilder().AddRequirements(new OlderThanRequirement(10)).Build();
                _options.AddPolicy(policyName, policyExists);
            }

            return policyExists;
        }
    }
}