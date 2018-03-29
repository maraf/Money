using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Money.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Data
{
    public static class ApplicationDataSeeder
    {
        public static IWebHost SeedData(this IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetService<UserManager<ApplicationUser>>();

                if (!userManager.Users.Any())
                    userManager.CreateAsync(new ApplicationUser("demo"), "demo").Wait();
            }

            return host;
        }
    }
}
