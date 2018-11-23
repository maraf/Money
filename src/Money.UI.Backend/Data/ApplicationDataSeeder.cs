using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Money.Commands;
using Money.Models;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
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
                var db = services.GetService<ApplicationDbContext>();

                db.Database.EnsureCreated();

                if (!userManager.Users.Any())
                    userManager.CreateAsync(new ApplicationUser(ClaimsPrincipalExtensions.DemoUserName), ClaimsPrincipalExtensions.DemoUserPassword).Wait();
            }

            return host;
        }

        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, ICommandDispatcher commands)
        {
            IdentityResult userResult = await userManager.CreateAsync(
                new ApplicationUser(ClaimsPrincipalExtensions.DemoUserName),
                ClaimsPrincipalExtensions.DemoUserPassword
            );

            if (!userResult.Succeeded)
                throw Ensure.Exception.InvalidOperation("Unnable to create demo user.");

            ApplicationUser user = await userManager.FindByNameAsync(ClaimsPrincipalExtensions.DemoUserName);
            if (user == null)
                throw Ensure.Exception.InvalidOperation("Unnable find created demo user.");

            IKey userKey = StringKey.Create(user.Id, "User");

            await commands.HandleAsync(WrapCommand(userKey, new CreateCurrency("USD", "$")));
            await commands.HandleAsync(WrapCommand(userKey, new CreateCategory("Car", "Gas etc.", Color.FromArgb(255, 145, 206, 234))));
            await commands.HandleAsync(WrapCommand(userKey, new CreateCategory("Home", "DIY", Color.FromArgb(255, 207, 180, 141))));
            await commands.HandleAsync(WrapCommand(userKey, new CreateCategory("Food", "Ingredients for home made meals", Color.FromArgb(255, 155, 237, 144))));
        }

        private static Envelope<T> WrapCommand<T>(IKey userKey, T command)
        {
            var envelope = Envelope.Create(command);
            envelope.Metadata.Add("UserKey", userKey);
            return envelope;
        }
    }
}
