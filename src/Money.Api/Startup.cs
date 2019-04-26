using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Money.Hubs;
using Money.Models;
using Money.Models.Api;
using Money.Users.Data;
using Money.Users.Models;

namespace Money
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConnectionStrings connectionStrings = Configuration
                .GetSection("ConnectionStrings")
                .Get<ConnectionStrings>();

            string ApplyBasePath(string value) => value.Replace("{BasePath}", Environment.ContentRootPath);

            connectionStrings.Application = ApplyBasePath(connectionStrings.Application);
            connectionStrings.EventSourcing = ApplyBasePath(connectionStrings.EventSourcing);
            connectionStrings.ReadModel = ApplyBasePath(connectionStrings.ReadModel);

            services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionStrings.Application));

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    JwtOptions configuration = Configuration.GetSection("Jwt").Get<JwtOptions>();

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration.Issuer,
                        ValidAudience = configuration.Issuer,
                        IssuerSigningKey = configuration.GetSecurityKey()
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/api"))
                            {
                                var accessToken = context.HttpContext.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(accessToken))
                                    context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };

                    options.SaveToken = true;
                });

            services
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                });

            services
                .AddIdentityCore<ApplicationUser>(options => Configuration.GetSection("Identity").GetSection("Password").Bind(options.Password))
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services
                .AddRouting(options => options.LowercaseUrls = true)
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddSignalR();

            services
                .AddTransient<JwtSecurityTokenHandler>()
                .Configure<JwtOptions>(Configuration.GetSection("Jwt"));

            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IUserIdProvider>(new DefaultUserIdProvider())
                .AddSingleton<ApiHub>()
                .AddSingleton<CommandMapper>()
                .AddSingleton<QueryMapper>();

            Bootstrap.BootstrapTask bootstrapTask = new Bootstrap.BootstrapTask(services, connectionStrings);
            bootstrapTask.Initialize();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseStatusCodePages();

            app.UseCors(p =>
            {
#if DEBUG
                p.WithOrigins("http://localhost:48613");
#else
                p.WithOrigins("https://api.money.neptuo.com");
#endif
                p.AllowAnyMethod();
                p.AllowCredentials();
                p.AllowAnyHeader();
                p.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
            });

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ApiHub>("/api");
            });

            app.UseMvc();
        }
    }
}
