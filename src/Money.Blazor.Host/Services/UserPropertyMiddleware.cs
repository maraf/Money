using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal class UserPropertyMiddleware : HttpQueryDispatcher.IMiddleware,
        IEventHandler<UserPropertyChanged>,
        IEventHandler<UserSignedOut>
    {
        private static readonly ListUserProperty listAllQuery = new ListUserProperty();

        private readonly List<UserPropertyModel> models = new List<UserPropertyModel>();
        private readonly ServerConnectionState serverConnection;
        private readonly UserPropertyStorage localStorage;
        private Task listAllTask;

        public UserPropertyMiddleware(ServerConnectionState serverConnection, UserPropertyStorage localStorage)
        {
            Ensure.NotNull(serverConnection, "serverConnection");
            Ensure.NotNull(localStorage, "localStorage");
            this.serverConnection = serverConnection;
            this.localStorage = localStorage;
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is ListUserProperty listAll)
            {
                await EnsureListAsync(null, next, listAllQuery);
                return models.Select(c => c.Clone()).ToList();
            }
            else if(query is FindUserProperty find)
            {
                await EnsureListAsync(dispatcher, null, listAllQuery);
                UserPropertyModel model = models.FirstOrDefault(m => m.Key == find.Key);
                if (model != null)
                    return model.Value;
            }

            return await next(query);
        }

        private async Task EnsureListAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListUserProperty listAll)
        {
            if (models.Count == 0)
            {
                if (listAllTask == null)
                    listAllTask = LoadAllAsync(dispatcher, next, listAll);

                try
                {
                    await listAllTask;
                }
                finally
                {
                    listAllTask = null;
                }
            }
        }

        private async Task LoadAllAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListUserProperty listAll)
        {
            models.Clear();
            if (!serverConnection.IsAvailable)
            {
                var items = await localStorage.LoadAsync();
                if (items != null)
                {
                    models.AddRange(items);
                    return;
                }
            }

            if (dispatcher != null)
            {
                await dispatcher.QueryAsync(listAll);
            }
            else
            {
                models.AddRange((List<UserPropertyModel>)await next(listAll));
                await localStorage.SaveAsync(models);
            }
        }

        async Task IEventHandler<UserPropertyChanged>.HandleAsync(UserPropertyChanged payload)
        {
            UserPropertyModel model = models.FirstOrDefault(c => c.Key == payload.PropertyKey);
            if (model == null)
                models.Add(model = new UserPropertyModel(payload.PropertyKey, null));

            model.Value = payload.Value;
            await localStorage.SaveAsync(models);
        }

        async Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            models.Clear();
            await localStorage.DeleteAsync();
        }
    }
}
