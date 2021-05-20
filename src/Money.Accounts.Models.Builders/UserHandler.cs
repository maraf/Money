using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Money.Commands;
using Money.Events;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Activators;
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

namespace Money.Models.Builders
{
    public class UserHandler : ICommandHandler<Envelope<ChangePassword>>,
        ICommandHandler<Envelope<ChangeEmail>>,
        IQueryHandler<GetProfile, ProfileModel>,
        ICommandHandler<Envelope<SetUserProperty>>,
        IQueryHandler<ListUserProperty, List<UserPropertyModel>>,
        IQueryHandler<FindUserProperty, string>
    {
        private readonly UserManager<User> userManager;
        private readonly IFactory<AccountContext> dbFactory;
        private readonly IEventDispatcher eventDispatcher;
        private readonly IReadOnlyCollection<string> allowPropertyKeys;

        public UserHandler(UserManager<User> userManager, IFactory<AccountContext> dbFactory, IEventDispatcher eventDispatcher, IReadOnlyCollection<string> allowPropertyKeys)
        {
            Ensure.NotNull(userManager, "userManager");
            Ensure.NotNull(dbFactory, "dbFactory");
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            Ensure.NotNull(allowPropertyKeys, "allowPropertyKeys");
            this.userManager = userManager;
            this.dbFactory = dbFactory;
            this.eventDispatcher = eventDispatcher;
            this.allowPropertyKeys = allowPropertyKeys;
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

        private async Task<(User Model, StringKey Key)> GetUserAsync(Envelope envelope)
        {
            StringKey userKey = envelope.Metadata.Get<StringKey>("UserKey");
            return (await GetUserAsync(userKey), userKey);
        }

        private async Task<User> GetUserAsync(StringKey userKey)
        {
            User model = await userManager.FindByIdAsync(userKey.Identifier);
            if (model == null)
                throw new InvalidOperationException($"Unable to load user with ID '{userKey.Identifier}'.");

            return model;
        }

        private void EnsureNotDemo<T>(Envelope<T> envelope, (User Model, StringKey Key) user)
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

        async Task ICommandHandler<Envelope<SetUserProperty>>.HandleAsync(Envelope<SetUserProperty> command)
        {
            using (var db = dbFactory.Create())
            {
                var user = await GetUserAsync(command);
                UserPropertyValue value = await FindUserPropertyValueAsync(db, user.Key, command.Body.PropertyKey);
                if (value == null)
                {
                    if (command.Body.Value == null)
                        return;

                    if (!allowPropertyKeys.Contains(command.Body.PropertyKey))
                        throw DecorateException(command.Body, user.Key, new UserPropertyNotSupportedException(command.Body.PropertyKey));

                    value = new UserPropertyValue()
                    {
                        UserId = user.Model.Id,
                        Key = command.Body.PropertyKey,
                        Value = command.Body.Value
                    };

                    await db.UserProperties.AddAsync(value);
                }
                else
                {
                    if (value.Value == command.Body.Value)
                        return;

                    if (command.Body.Value == null)
                        db.UserProperties.Remove(value);
                    else
                        value.Value = command.Body.Value;
                }

                await db.SaveChangesAsync();
                await eventDispatcher.PublishAsync(new UserPropertyChanged(GuidKey.Create(Guid.NewGuid(), "UserProperty"), user.Key, command.Body.PropertyKey, command.Body.Value));
            }
        }

        async Task<List<UserPropertyModel>> IQueryHandler<ListUserProperty, List<UserPropertyModel>>.HandleAsync(ListUserProperty query)
        {
            List<UserPropertyModel> result = new List<UserPropertyModel>();
            using (var db = dbFactory.Create())
            {
                var values = await db.UserProperties.Where(v => v.User.Id == query.UserKey.AsStringKey().Identifier)
                    .Select(v => new { Key = v.Key, Value = v.Value })
                    .ToListAsync();

                foreach (var key in allowPropertyKeys)
                    result.Add(new UserPropertyModel(key, values.FirstOrDefault(v => v.Key == key)?.Value));
            }

            return result;
        }

        async Task<string> IQueryHandler<FindUserProperty, string>.HandleAsync(FindUserProperty query)
        {
            using (var db = dbFactory.Create())
            {
                UserPropertyValue value = await FindUserPropertyValueAsync(db, query.UserKey, query.Key);
                return value?.Value;
            }
        }

        private static async Task<UserPropertyValue> FindUserPropertyValueAsync(AccountContext db, IKey userKey, string propertyKey)
        {
            return await db.UserProperties
                .Where(v => v.UserId == userKey.AsStringKey().Identifier && v.Key == propertyKey)
                .FirstOrDefaultAsync();
        }
    }
}
