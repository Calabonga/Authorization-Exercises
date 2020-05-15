using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.Client.Mvc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
                {
                    config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = "oidc";
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect("oidc", config =>
                {
                    config.Authority = "https://localhost:10001";
                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secret_mvc";
                    config.SaveTokens = true;

                    config.ResponseType = "code";

                    config.Scope.Add("OrdersAPI");
                    config.Scope.Add("offline_access");

                    config.GetClaimsFromUserInfoEndpoint = true;

                    config.ClaimActions.MapJsonKey(ClaimTypes.DateOfBirth, ClaimTypes.DateOfBirth);
                });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("HasDateOfBirth", builder =>
                {
                    builder.RequireClaim(ClaimTypes.DateOfBirth);
                });

                //config.AddPolicy("OlderThan", builder =>
                //{
                //    builder.AddRequirements(new OlderThanRequirement(10));
                //});
            });

            services.AddSingleton<IAuthorizationHandler, OlderThanRequirementHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

            services.AddHttpClient();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

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


    public class OlderThanRequirement : IAuthorizationRequirement
    {
        public OlderThanRequirement(int years)
        {
            Years = years;
        }

        public int Years { get; }
    }

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
