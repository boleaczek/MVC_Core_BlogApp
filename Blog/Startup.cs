using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Models.Other;
using Blog.UnitsOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Blog.Security;

namespace Blog
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                                        .RequireAuthenticatedUser()
                                        .Build();
                })
                 .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddTransient<BlogData>();
            services.AddTransient<IBlogUnitOfWork, BlogUnitOfWork>();

            string mainDbConnectionString;
            string userDbConnectionString;

            mainDbConnectionString = "mainDB";
            userDbConnectionString = "userDB";


            services.AddDbContext<BlogContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString(mainDbConnectionString)));
            services.AddDbContext<UserContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(userDbConnectionString)));

            services.BuildServiceProvider().GetService<BlogContext>().Database.Migrate();
            services.BuildServiceProvider().GetService<UserContext>().Database.Migrate();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<UserContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ISecurityFacade, SecurityFacade>();
            services.AddTransient<IAccountUnitOfWork, AccountUnitOfWork>();

            services.AddCors(options =>
            {
                options.AddPolicy("AjaxCommentsPolicy", builder =>
                    {
                        builder.WithOrigins(Configuration.GetValue<string>("Host"))
                            .AllowAnyHeader()
                            .WithMethods("GET")
                            .AllowCredentials();
                    });
            });

            services.AddSingleton<IAuthorizationHandler, BlogAdminAuthorizationHandler>();
        }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseCors("AjaxCommentsPolicy");
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
