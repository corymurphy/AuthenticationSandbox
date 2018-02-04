using AuthenticationSandbox.Web.Data;
using AuthenticationSandbox.Web.Models;
using AuthenticationSandbox.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationSandbox.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            


            services.AddAuthentication().AddWsFederation
                (
                    options =>
                    {
                        options.MetadataAddress = Configuration.GetValue<string>("WsFederationMetadataUrl");
                        options.Wtrealm = Configuration.GetValue<string>("WsFederationAudience");
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidAudience = Configuration.GetValue<string>("WsFederationAudience")
                        };

                    }
                );

            services.AddAuthentication().AddOpenIdConnect
                (
                    options =>
                    {
                        options.Authority = "https://labidp.sadep.local/SecureAuth18";
                        options.ClientId = "50893151dc864376942d8b00266d144d";
                        options.Scope.Add("email");
                    }
                );
            
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //app.UseOpenIdConnectAuthentication(new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectOptions());


            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
