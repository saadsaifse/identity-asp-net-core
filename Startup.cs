using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspIdentity.Infrastructure;
using AspIdentity.Models;
using AspIdentity.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspIdentity
{
    public class Startup
    {
        readonly IConfiguration configuration;
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IClaimsTransformation, LocationClaimsProvider>();
            services.AddTransient<IAuthorizationHandler, BlockUserHandler>();
            services.AddTransient<IAuthorizationHandler, ProtectCaseHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("DCStatePolicy", policy => policy
                            .RequireRole("Admins")
                            .RequireClaim(ClaimTypes.StateOrProvince, "DC"));
                
                options.AddPolicy("BlockSaad", policy => policy
                            .RequireAuthenticatedUser()
                            .AddRequirements(new BlockUserRequirement("Saad")));

                options.AddPolicy("TrimCases", policy=>policy
                            .RequireAuthenticatedUser()
                            .AddRequirements(new ProtectCasesRequirement(false)));
            });

            services.AddAuthentication().AddGoogle(opts => 
            {
                opts.ClientId = "yourClientId.apps.googleusercontent.com";        
                opts.ClientSecret = "yourSecret";
            });

            services.AddDbContext<AppIdentityDbContext>(options =>
                    options.UseSqlServer(configuration["Data:Identity:ConnectionString"]));

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
