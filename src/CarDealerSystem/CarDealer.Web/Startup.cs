namespace CarDealer.Web
{
    using AutoMapper;
    using CarDealer.Models;
    using Data;
    using Infrastructure.Extensions;
    using Infrastructure.Middleware;
    using Infrastructure.Utilities;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<SendGridOptions>(this.Configuration.GetSection("EmailSetting"))
                .Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<CarDealerDbContext>(options =>
                options.UseSqlServer(
                    this.Configuration.GetConnectionString("CarDealer")));

            services
                .AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;

                    options.SignIn.RequireConfirmedEmail = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddDefaultUI()
                .AddEntityFrameworkStores<CarDealerDbContext>()
                .AddDefaultTokenProviders();

            services
                .AddDistributedMemoryCache();

            services
                .AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = this.Configuration.GetSection("ExternalAuthentications:Facebook:AppId").Value;
                    facebookOptions.AppSecret = this.Configuration.GetSection("ExternalAuthentications:Facebook:AppSecret").Value;
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = this.Configuration.GetSection("ExternalAuthentications:Google:ClientId").Value;
                    googleOptions.ClientSecret = this.Configuration.GetSection("ExternalAuthentications:Google:ClientSecret").Value;
                });

            services
                .ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = $"/Identity/Account/Login";
                    options.LogoutPath = $"/Identity/Account/Logout";
                    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                });

            services
                .AddApplicationServices()
                .AddDomainServices()
                .AddAutoMapper()
                .AddResponseCompression(options => options.EnableForHttps = true);
            
            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                })
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.AddDefaultSecurityHeaders(
                new SecurityHeadersBuilder()
                    .AddDefaultSecurePolicy());
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(name: "customAdAreaRoute",
                    areaName: "ad",
                    template: "{controller=Ad}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.SeedData();
        }
    }
}
