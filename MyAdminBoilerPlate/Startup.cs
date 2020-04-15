using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.Security;

namespace MyAdminBoilerPlate
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options => {    
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 3;
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

            //services.Configure<IdentityOptions>(options => {
            //    options.Password.RequireDigit = true;
            //    options.Password.RequiredLength = 3;
            //});

            services.Configure<DataProtectionTokenProviderOptions>(
                o => o.TokenLifespan = TimeSpan.FromHours(5)
            );

            services.Configure<CustomEmailConfirmationTokenProviderOptions>(
                o => o.TokenLifespan = TimeSpan.FromDays(3)
            );

            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("con")));
            services.AddMvc();
            services.AddScoped<IUserRepository, SQLUserRepository>();

            services.AddAuthorization(options =>
            {
                // claims based authorization policy
                options.AddPolicy("ManageRolePolicy", policy => 
                                                      policy.RequireClaim("Delete Role")
                                                            .RequireClaim("Create Role")
                );

                options.AddPolicy("EditRolePolicy", policy => 
                                                    policy.RequireAssertion(context => 
                                                    context.User.IsInRole("Admin") &&
                                                    context.User.HasClaim(claim => 
                                                                          claim.Type == "Edit Role" && 
                                                                          claim.Value == "true") ||
                                                     context.User.IsInRole("Super Admin")
                                                    )
                );

                options.AddPolicy("CustomPolicy", policy => 
                                                  policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement())
                );

                // you can use policy to add role base autorization too
                options.AddPolicy("AdminRolePloicy", policy => 
                                                     policy.RequireRole("Super Admin", "Admin")
                );
            });

            services.AddSingleton<IAuthorizationHandler, CanEditOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
            services.AddSingleton<DataProtectionPurposeStrings>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // if you want to customise your developer exception page
                // use "DeveloperExceptionPageOptions"
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            // FileServerOptions combines the functionality of:
            // useDefaultFiles, useStaticFiles, directoryBrowser middleware.
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("foo.html");
            app.UseDefaultFiles(defaultFilesOptions);
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run(async (context) =>
            {
                // the below error will never run except if a default file is not found
                throw new Exception("Some error occured during processing");
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
