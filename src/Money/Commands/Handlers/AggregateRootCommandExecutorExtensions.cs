using Money.Events;
using Neptuo;
using Neptuo.Commands.Handlers;
using Neptuo.Events;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands.Handlers
{
    internal static class AggregateRootCommandExecutorExtensions
    {
        public static Task Execute<T, TRepository>(this AggregateRootCommandExecutor<T, TRepository> executor, IKey aggregateKey, Envelope envelope, Action<T, IKey> handler)
            where T : AggregateRoot
            where TRepository : IRepository<T, IKey>
        {
            return executor.Execute(aggregateKey, model =>
            {
                IKey userKey;
                if (!envelope.Metadata.TryGet("UserKey", out userKey))
                {
                    if (envelope.Metadata.TryGet("UserId", out string userId))
                        userKey = StringKey.Create(userId, "User");
                    else
                        userKey = StringKey.Empty("User");
                }

                handler(model, userKey);

                foreach (IEvent item in model.Events)
                {
                    if (item is UserEvent payload)
                        payload.UserKey = userKey;
                }
            });
        }

        public static Task Execute<T, TRepository>(this AggregateRootCommandExecutor<T, TRepository> executor, IKey aggregateKey, Envelope envelope, Action<T> handler)
            where T : AggregateRoot
            where TRepository : IRepository<T, IKey>
        {
            return Execute(executor, aggregateKey, envelope, (model, userKey) => handler(model));
        }


        public static Task Execute<T, TRepository>(this AggregateRootCommandExecutor<T, TRepository> executor, Envelope envelope, Func<IKey, T> handler)
            where T : AggregateRoot
            where TRepository : IRepository<T, IKey>
        {
            return executor.Execute(() =>
            {
                IKey userKey;
                if (!envelope.Metadata.TryGet("UserKey", out userKey))
                {
                    if (envelope.Metadata.TryGet("UserId", out string userId))
                        userKey = StringKey.Create(userId, "User");
                    else
                        userKey = StringKey.Empty("User");
                }

                T model = handler(userKey);

                foreach (IEvent item in model.Events)
                {
                    if (item is UserEvent payload)
                        payload.UserKey = userKey;
                }

                return model;
            });
        }

        public static Task Execute<T, TRepository>(this AggregateRootCommandExecutor<T, TRepository> executor, Envelope envelope, Func<T> handler)
            where T : AggregateRoot
            where TRepository : IRepository<T, IKey>
        {
            return Execute(executor, envelope, userKey => handler());
        }
    }
}
