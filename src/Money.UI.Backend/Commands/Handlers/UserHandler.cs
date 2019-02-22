using Microsoft.AspNetCore.Identity;
using Money.Events;
using Money.Models;
using Neptuo;
using Neptuo.Collections.Specialized;
using Neptuo.Commands.Handlers;
using Neptuo.Events;
using Neptuo.Models;
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
        private readonly IEventDispatcher eventDispatcher;

        public UserHandler(UserManager<ApplicationUser> userManager, IEventDispatcher eventDispatcher)
        {
            Ensure.NotNull(userManager, "userManager");
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            this.userManager = userManager;
            this.eventDispatcher = eventDispatcher;
        }

        public async Task HandleAsync(Envelope<ChangePassword> command)
        {
            StringKey userKey = command.Metadata.Get<StringKey>("UserKey");
            ApplicationUser user = await userManager.FindByIdAsync(userKey.Identifier);
            if (user == null)
                throw new InvalidOperationException($"Unable to load user with ID '{userKey.Identifier}'.");

            IdentityResult result = await userManager.ChangePasswordAsync(user, command.Body.Current, command.Body.New);
            if (!result.Succeeded)
            {
                var ex = new PasswordChangeFailedException(String.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                AggregateRootExceptionDecorator decorator = new AggregateRootExceptionDecorator(ex);
                decorator.SetCommandKey(command.Body.Key);
                decorator.SetSourceCommandKey(command.Body.Key);
                decorator.SetKey(userKey);
                throw ex;
            }

            await eventDispatcher.PublishAsync(new PasswordChanged(GuidKey.Create(Guid.NewGuid(), "PasswordChanged"), userKey));
        }
    }
}
