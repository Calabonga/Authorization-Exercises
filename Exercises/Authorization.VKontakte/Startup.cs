using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Authorization.VKontakte.Data;
using Authorization.VKontakte.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Authorization.VKontakte
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(config =>
                {
                    config.UseInMemoryDatabase("MEMORY");
                })
                .AddIdentity<ApplicationUser, ApplicationRole>(config =>
                {
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
                .AddFacebook(config =>
                {
                    config.AppId = _configuration["Authentication:Facebook:AppId"];
                    config.AppSecret = _configuration["Authentication:Facebook:AppSecret"];
                })
                .AddOAuth("VK", "VKontakte", config =>
                {
                    config.ClientId =  _configuration["Authentication:VK:AppId"];
                    config.ClientSecret =  _configuration["Authentication:VK:AppSecret"];
                    config.ClaimsIssuer = "VKontakte";
                    config.CallbackPath = new PathString("/signin-vkontakte-token");
                    config.AuthorizationEndpoint = "https://oauth.vk.com/authorize";
                    config.TokenEndpoint = "https://oauth.vk.com/access_token";
                    config.Scope.Add("email");
                    config.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
                    config.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    config.SaveTokens = true;
                    config.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            context.RunClaimActions(context.TokenResponse.Response.RootElement);
                            return Task.CompletedTask;
                        },
                        OnRemoteFailure = OnFailure
                    };
                });

            services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Admin/Login";
                config.AccessDeniedPath = "/Home/AccessDenied";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, "Administrator");
                });

                options.AddPolicy("Manager", builder =>
                {
                    builder.RequireAssertion(x => x.User.HasClaim(ClaimTypes.Role, "Manager")
                                                  || x.User.HasClaim(ClaimTypes.Role, "Administrator"));
                });

            });

            services.AddControllersWithViews();
        }

        private Task OnFailure(RemoteFailureContext arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
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
}
