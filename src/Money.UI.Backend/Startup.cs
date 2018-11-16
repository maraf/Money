using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Money.Data;
using Money.Hubs;
using Money.Models;
using Money.Models.Api;
using Money.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Money
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
            var connectionStrings = Configuration.GetSection("ConnectionStrings").Get<Bootstrap.ConnectionStrings>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionStrings.Application)
            );

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });

            services.AddAuthentication().AddCookie();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Api", policy =>
                {
                    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options => Configuration.GetSection("Identity").GetSection("Password").Bind(options.Password))
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc();
            services.AddSignalR();

            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<NavigatorUrl>()
                .AddSingleton<ApiHub>()
                .AddSingleton<CommandMapper>()
                .AddSingleton<QueryMapper>();

            Bootstrap.BootstrapTask bootstrapTask = new Bootstrap.BootstrapTask(services, connectionStrings);
            bootstrapTask.Initialize();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ApiHub>("/api");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{Action=Index}"
                );
            });

            app.Use(async (context, next) =>
            {
                var service = context.RequestServices.GetRequiredService<IAuthorizationService>();
                var result = await service.AuthorizeAsync(context.User, context, "Api");
                if (result.Succeeded)
                    await next();
                else
                    await context.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            });

            app.UseBlazor<UI.Blazor.Program>();
        }
    }
}
