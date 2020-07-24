using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Authorization.IdentityServer.Data;
using Authorization.IdentityServer.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.IdentityServer
{
    public class Startup
    {
        private readonly IHostEnvironment _environment;

        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(config =>
                {
                    config.UseSqlServer(Configuration.GetConnectionString(nameof(ApplicationDbContext)));
                })
                .AddIdentity<IdentityUser, IdentityRole>(config =>
                {
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(config =>
            {
                //config.LoginPath = "/Auth/Login";
                //config.LogoutPath = "/Auth/Logout";
                config.Cookie.Name = "IdentityServer.Cookies";
            });

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // var filePath = Path.Combine(_environment.ContentRootPath, "IdentityServer4_certificate.pfx");
            // var certificate = new X509Certificate2(filePath, "P@55w0rd");

            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = b =>
                //        b.UseSqlServer(Configuration.GetConnectionString(nameof(ApplicationDbContext)),
                //            sql => sql.MigrationsAssembly(migrationsAssembly));
                //})
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = b =>
                //        b.UseSqlServer(Configuration.GetConnectionString(nameof(ApplicationDbContext)),
                //            sql => sql.MigrationsAssembly(migrationsAssembly));
                //})
                .AddInMemoryClients(IdentityServerConfiguration.GetClients())
                .AddInMemoryApiResources(IdentityServerConfiguration.GetApiResources())
                .AddInMemoryIdentityResources(IdentityServerConfiguration.GetIdentityResources())
                .AddInMemoryApiScopes(IdentityServerConfiguration.GetApiScopes()) // IdentityServer4 version 4.x.x changes
                .AddProfileService<ProfileService>()
                // .AddSigningCredential(certificate);
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
