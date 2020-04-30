using Money.Events;
using Money.Models;
using Money.Queries;
using Neptuo;
using Neptuo.Events.Handlers;
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

        private ProfileModel profile;
        private Task getProfileTask;

        public UserMiddleware(ServerConnectionState serverConnection, ProfileStorage localStorage)
        {
            Ensure.NotNull(serverConnection, "serverConnection");
            Ensure.NotNull(localStorage, "localStorage");
            this.serverConnection = serverConnection;
            this.localStorage = localStorage;
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is GetProfile getProfile)
            {
                if (profile == null)
                {
                    if (getProfileTask == null)
                        getProfileTask = LoadProfileAsync(getProfile, next);

                    await getProfileTask;
                    getProfileTask = null;
                }

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
