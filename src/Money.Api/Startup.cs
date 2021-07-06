using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Money.Accounts;
using Money.Accounts.Middlewares;
using Money.Accounts.Models;
using Money.Api;
using Money.Api.Routing;
using Money.Common.Diagnostics;
using Money.EntityFrameworkCore;
using Money.Hubs;
using Money.Models;
using Money.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        private string ApplyBasePath(string value) 
            => value.Replace("{BasePath}", Environment.ContentRootPath);

        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration connectionStrings = Configuration.GetSection("Database");

            services
                .AddDbContextWithSchema<AccountContext>(connectionStrings.GetSection("Application"), ApplyBasePath);

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
                                string accessToken = context.HttpContext.Request.Query["access_token"].FirstOrDefault();
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
                .AddIdentityCore<User>(options => Configuration.GetSection("Identity").GetSection("Password").Bind(options.Password))
                .AddEntityFrameworkStores<AccountContext>();

            services
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers()
                .AddNewtonsoftJson();

            services
                .AddVersionHeader();

            services
                .AddSignalR();

            services
                .AddHealthChecks()
                .AddDbContextCheck<AccountContext>()
                .AddFactoryDbContextCheck<ReadModelContext>()
                .AddFactoryDbContextCheck<EventSourcingContext>();

            services
                .AddTransient<JwtTokenGenerator>()
                .AddTransient<JwtSecurityTokenHandler>()
                .Configure<JwtOptions>(Configuration.GetSection("Jwt"));

            services
                .AddSingleton<Json>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IUserIdProvider>(new DefaultUserIdProvider())
                .AddTransient<ExceptionMiddleware>()
                .AddTransient<RenewableTokenMiddleware>()
                .AddSingleton<ApiHub>()
                .AddSingleton<CommandMapper>()
                .AddSingleton<QueryMapper>();

            var allowedUserPropertyKeys = Configuration.GetSection("UserProperties").Get<string[]>() ?? new string[0];
            Bootstrap.BootstrapTask bootstrapTask = new Bootstrap.BootstrapTask(services, connectionStrings, allowedUserPropertyKeys, ApplyBasePath);
            bootstrapTask.Initialize();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePages();
                app.UseErrorHandler();
            }

            app.UseRouting();

            app.UseCors(p =>
            {
#if DEBUG
                p.WithOrigins("http://localhost:48613");
#else
                p.WithOrigins("https://app.money.neptuo.com", "https://beta.app.money.neptuo.com");
#endif
                p.AllowAnyMethod();
                p.AllowCredentials();
                p.AllowAnyHeader();
                p.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                p.WithExposedHeaders(VersionHeader.Name);
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRenewableToken();

            app.UseVersionHeader();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapHub<ApiHub>("/api");
                endpoints.MapControllers();
            });
        }
    }
}
