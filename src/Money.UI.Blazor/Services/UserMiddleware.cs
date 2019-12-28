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
        private readonly NetworkState network;
        private readonly ProfileStorage localStorage;

        private ProfileModel profile;
        private Task getProfileTask;

        public UserMiddleware(NetworkState network, ProfileStorage localStorage)
        {
            Ensure.NotNull(network, "network");
            Ensure.NotNull(localStorage, "localStorage");
            this.network = network;
            this.localStorage = localStorage;
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is GetProfile getProfile)
            {
                if (profile == null)
                {
                    if (getProfileTask == null)
                        getProfileTask = LoadProfileAsync(getProfile, next).ContinueWith(t => getProfileTask = null);

                    await getProfileTask;
                }

                return profile;
            }

            return await next(query);
        }

        private async Task LoadProfileAsync(GetProfile query, HttpQueryDispatcher.Next next)
        {
            if (!network.IsOnline)
            {
                profile = await localStorage.LoadAsync();
                if (profile != null)
                    return;
            }

            profile = (ProfileModel)await next(query);
            await localStorage.SaveAsync(profile);
        }

        Task IEventHandler<EmailChanged>.HandleAsync(EmailChanged payload)
        {
            if (profile != null)
                profile.Email = payload.Email;

            return Task.CompletedTask;
        }

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            profile = null;

            return Task.CompletedTask;
        }
    }
}
