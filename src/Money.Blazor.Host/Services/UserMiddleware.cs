using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal class UserMiddleware : HttpQueryDispatcher.IMiddleware,
        IEventHandler<EmailChanged>,
        IEventHandler<UserSignedOut>
    {
        private readonly ServerConnectionState serverConnection;
        private readonly ProfileStorage localStorage;
        private readonly ILog log;

        private ProfileModel profile;
        private Task getProfileTask;

        public UserMiddleware(ServerConnectionState serverConnection, ProfileStorage localStorage, ILogFactory logFactory)
        {
            Ensure.NotNull(serverConnection, "serverConnection");
            Ensure.NotNull(localStorage, "localStorage");
            Ensure.NotNull(logFactory, "logFactory");
            this.serverConnection = serverConnection;
            this.localStorage = localStorage;
            this.log = logFactory.Scope("UserMiddleware");
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is GetProfile getProfile)
            {
                if (profile == null)
                {
                    log.Debug("Profile is null.");

                    if (getProfileTask == null)
                    {
                        log.Debug("Profile task is null.");
                        getProfileTask = LoadProfileAsync(getProfile, next);
                    }

                    try
                    {
                        log.Debug("Awating profile task.");
                        await getProfileTask;
                    }
                    finally
                    {
                        log.Debug("Clearing profile task.");
                        getProfileTask = null;
                    }
                }

                log.Debug("Returning profile.");
                return profile;
            }

            return await next(query);
        }

        private async Task LoadProfileAsync(GetProfile query, HttpQueryDispatcher.Next next)
        {
            if (!serverConnection.IsAvailable)
            {
                profile = await localStorage.LoadAsync();
                if (profile != null)
                    return;
            }

            log.Debug("Get profile over the wire.");
            profile = (ProfileModel)await next(query);
            await localStorage.SaveAsync(profile);
        }

        async Task IEventHandler<EmailChanged>.HandleAsync(EmailChanged payload)
        {
            if (profile != null)
                profile.Email = payload.Email;

            await localStorage.SaveAsync(profile);
        }

        async Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            profile = null;
            await localStorage.DeleteAsync();
        }
    }
}
