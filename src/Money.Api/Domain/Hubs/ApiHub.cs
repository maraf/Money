using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Money.Api.Models;
using Money.Events;
using Money.Services;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters;
using Neptuo.Models;
using Neptuo.Models.Keys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Money.Hubs
{
    [Authorize]
    public class ApiHub : Hub,
        IExceptionHandler<AggregateRootException>,
        IEventHandler<CategoryCreated>, IEventHandler<CategoryDeleted>, IEventHandler<CategoryRenamed>, IEventHandler<CategoryDescriptionChanged>, IEventHandler<CategoryIconChanged>, IEventHandler<CategoryColorChanged>,
        IEventHandler<CurrencyCreated>, IEventHandler<CurrencyDeleted>, IEventHandler<CurrencyDefaultChanged>, IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyExchangeRateSet>, IEventHandler<CurrencyExchangeRateRemoved>,
        IEventHandler<OutcomeCreated>, IEventHandler<OutcomeDeleted>, IEventHandler<OutcomeAmountChanged>, IEventHandler<OutcomeDescriptionChanged>, IEventHandler<OutcomeWhenChanged>,
        IEventHandler<ExpenseTemplateCreated>, IEventHandler<ExpenseTemplateDeleted>,
        IEventHandler<IncomeCreated>, IEventHandler<IncomeAmountChanged>, IEventHandler<IncomeDescriptionChanged>, IEventHandler<IncomeWhenChanged>, IEventHandler<IncomeDeleted>,
        IEventHandler<PasswordChanged>, IEventHandler<EmailChanged>, IEventHandler<UserPropertyChanged>
    {
        private readonly FormatterContainer formatters;

        private readonly Dictionary<IKey, List<string>> userKeyToConnectionId = new Dictionary<IKey, List<string>>();
        private readonly object userKeyToConnectionIdLock = new object();

        private readonly Dictionary<IKey, IKey> commandKeyToUserKey = new Dictionary<IKey, IKey>();
        private readonly object commandKeyToUserKeyLock = new object();

        public ApiHub(IEventHandlerCollection eventHandlers, FormatterContainer formatters, ExceptionHandlerBuilder exceptionHandlerBuilder)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(formatters, "formatters");
            this.formatters = formatters;
            eventHandlers.AddAll(this);
            exceptionHandlerBuilder.Filter<AggregateRootException>().Handler(this);
        }

        private (string connectionId, IKey key) GetUserInfo()
        {
            string connectionId = Context.ConnectionId;
            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new UnauthorizedAccessException();

            IKey userKey = StringKey.Create(userId, "User");
            return (connectionId, userKey);
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var userInfo = GetUserInfo();

            lock (userKeyToConnectionIdLock)
            {
                if (!userKeyToConnectionId.TryGetValue(userInfo.key, out List<string> value))
                    userKeyToConnectionId[userInfo.key] = value = new List<string>();

                value.Add(userInfo.connectionId);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userInfo = GetUserInfo();
            lock (userKeyToConnectionIdLock)
            {
                if (userKeyToConnectionId.TryGetValue(userInfo.key, out List<string> value))
                {
                    if (value.Remove(userInfo.connectionId) && value.Count == 0)
                        userKeyToConnectionId.Remove(userInfo.key);
                }
            }

            return base.OnDisconnectedAsync(exception);
        }

        private Task RaiseEvent<T>(T payload)
        {
            string type = typeof(T).AssemblyQualifiedName;
            string rawPayload = formatters.Event.Serialize(payload);

            IClientProxy target = null;
            lock (userKeyToConnectionIdLock)
            {
                if (payload is UserEvent userEvent && userKeyToConnectionId.TryGetValue(userEvent.UserKey, out List<string> value))
                    target = Clients.Clients(value.ToList());
            }

            if (target != null)
            {
                return target.SendAsync(ApiHubMethod.RaiseEvent, JsonConvert.SerializeObject(new Response()
                {
                    Type = type,
                    Payload = rawPayload
                }));
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryCreated>.HandleAsync(CategoryCreated payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryDeleted>.HandleAsync(CategoryDeleted payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryRenamed>.HandleAsync(CategoryRenamed payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryDescriptionChanged>.HandleAsync(CategoryDescriptionChanged payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryIconChanged>.HandleAsync(CategoryIconChanged payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryColorChanged>.HandleAsync(CategoryColorChanged payload) => RaiseEvent(payload);

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload) => RaiseEvent(payload);
        Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload) => RaiseEvent(payload);
        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload) => RaiseEvent(payload);
        Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload) => RaiseEvent(payload);
        Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload) => RaiseEvent(payload);
        Task IEventHandler<CurrencyExchangeRateRemoved>.HandleAsync(CurrencyExchangeRateRemoved payload) => RaiseEvent(payload);

        Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload) => RaiseEvent(payload);
        Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload) => RaiseEvent(payload);
        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload) => RaiseEvent(payload);
        Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload) => RaiseEvent(payload);
        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload) => RaiseEvent(payload);

        Task IEventHandler<ExpenseTemplateCreated>.HandleAsync(ExpenseTemplateCreated payload) => RaiseEvent(payload);
        Task IEventHandler<ExpenseTemplateDeleted>.HandleAsync(ExpenseTemplateDeleted payload) => RaiseEvent(payload);

        Task IEventHandler<IncomeCreated>.HandleAsync(IncomeCreated payload) => RaiseEvent(payload);
        Task IEventHandler<IncomeAmountChanged>.HandleAsync(IncomeAmountChanged payload) => RaiseEvent(payload);
        Task IEventHandler<IncomeDescriptionChanged>.HandleAsync(IncomeDescriptionChanged payload) => RaiseEvent(payload);
        Task IEventHandler<IncomeWhenChanged>.HandleAsync(IncomeWhenChanged payload) => RaiseEvent(payload);
        Task IEventHandler<IncomeDeleted>.HandleAsync(IncomeDeleted payload) => RaiseEvent(payload);

        Task IEventHandler<PasswordChanged>.HandleAsync(PasswordChanged payload) => RaiseEvent(payload);
        Task IEventHandler<EmailChanged>.HandleAsync(EmailChanged payload) => RaiseEvent(payload);
        Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload) => RaiseEvent(payload);

        public void Handle(AggregateRootException exception)
        {
            string type = exception.GetType().AssemblyQualifiedName;
            string rawPayload = formatters.Exception.Serialize(exception);

            IClientProxy target = null;
            lock (commandKeyToUserKeyLock)
            {
                if (commandKeyToUserKey.TryGetValue(exception.FindOriginalCommandKey(), out IKey userKey))
                {
                    lock (userKeyToConnectionIdLock)
                    {
                        if (userKeyToConnectionId.TryGetValue(userKey, out List<string> value))
                            target = Clients.Clients(value.ToList());
                    }
                }
            }

            if (target != null)
            {
                target.SendAsync(ApiHubMethod.RaiseException, JsonConvert.SerializeObject(new Response()
                {
                    Type = type,
                    Payload = rawPayload
                }));
            }
        }

        public void AddCommand(IKey commandKey, IKey userKey)
        {
            Ensure.Condition.NotEmptyKey(commandKey);
            Ensure.Condition.NotEmptyKey(userKey);
            lock (commandKeyToUserKeyLock)
            {
                commandKeyToUserKey[commandKey] = userKey;
            }
        }
    }
}
