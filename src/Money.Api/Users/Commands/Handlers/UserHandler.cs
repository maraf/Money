using Microsoft.AspNetCore.Identity;
using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Queries;
using Money.Users.Models;
using Neptuo;
using Neptuo.Collections.Specialized;
using Neptuo.Commands;
using Neptuo.Commands.Handlers;
using Neptuo.Events;
using Neptuo.Models;
using Neptuo.Models.Keys;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Users.Commands.Handlers
{
    public class UserHandler : ICommandHandler<Envelope<ChangePassword>>, ICommandHandler<Envelope<ChangeEmail>>, IQueryHandler<GetProfile, ProfileModel>
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

        private T DecorateException<T>(ICommand command, StringKey userKey, T ex)
            where T : AggregateRootException
        {
            AggregateRootExceptionDecorator decorator = new AggregateRootExceptionDecorator(ex);
            decorator.SetCommandKey(command.Key);
            decorator.SetSourceCommandKey(command.Key);
            decorator.SetKey(userKey);
            return ex;
        }

        private async Task<(ApplicationUser Model, StringKey Key)> GetUserAsync(Envelope envelope)
        {
            StringKey userKey = envelope.Metadata.Get<StringKey>("UserKey");
            return (await GetUserAsync(userKey), userKey);
        }

        private async Task<ApplicationUser> GetUserAsync(StringKey userKey)
        {
            ApplicationUser model = await userManager.FindByIdAsync(userKey.Identifier);
            if (model == null)
                throw new InvalidOperationException($"Unable to load user with ID '{userKey.Identifier}'.");

            return model;
        }

        private void EnsureNotDemo<T>(Envelope<T> envelope, (ApplicationUser Model, StringKey Key) user)
            where T : ICommand
        {
            if (user.Model.IsDemo())
                throw DecorateException(envelope.Body, user.Key, new DemoUserCantBeChangedException());
        }

        public async Task HandleAsync(Envelope<ChangePassword> command)
        {
            var user = await GetUserAsync(command);
            EnsureNotDemo(command, user);

            var result = await userManager.ChangePasswordAsync(user.Model, command.Body.Current, command.Body.New);
            if (!result.Succeeded)
                throw DecorateException(command.Body, user.Key, new PasswordChangeFailedException(String.Join(Environment.NewLine, result.Errors.Select(e => e.Description))));

            await eventDispatcher.PublishAsync(new PasswordChanged(GuidKey.Create(Guid.NewGuid(), "PasswordChanged"), user.Key));
        }

        public async Task HandleAsync(Envelope<ChangeEmail> command)
        {
            var user = await GetUserAsync(command);
            EnsureNotDemo(command, user);

            var result = await userManager.SetEmailAsync(user.Model, command.Body.Email);
            if (!result.Succeeded)
                throw DecorateException(command.Body, user.Key, new EmailChangeFailedException());

            await eventDispatcher.PublishAsync(new EmailChanged(GuidKey.Create(Guid.NewGuid(), "EmailChanged"), user.Key, command.Body.Email));
        }

        public async Task<ProfileModel> HandleAsync(GetProfile query)
        {
            var user = await GetUserAsync(query.UserKey.AsStringKey());
            return new ProfileModel(user.UserName, user.Email);
        }
    }
}
