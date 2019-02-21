using Microsoft.AspNetCore.Identity;
using Money.Models;
using Neptuo;
using Neptuo.Collections.Specialized;
using Neptuo.Commands.Handlers;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands.Handlers
{
    public class UserHandler : ICommandHandler<Envelope<ChangePassword>>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserHandler(UserManager<ApplicationUser> userManager)
        {
            Ensure.NotNull(userManager, "userManager");
            this.userManager = userManager;
        }

        public async Task HandleAsync(Envelope<ChangePassword> command)
        {
            StringKey userKey = command.Metadata.Get<StringKey>("UserKey");
            ApplicationUser user = await userManager.FindByIdAsync(userKey.Identifier);
            if (user == null)
                throw new InvalidOperationException($"Unable to load user with ID '{userKey.Identifier}'.");

            IdentityResult result = await userManager.ChangePasswordAsync(user, command.Body.Current, command.Body.New);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Password change failed for ID '{userKey.Identifier}'.");
        }
    }
}
