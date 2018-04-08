using Money.Models.Queries;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    internal class UserQueryDispatcher : IQueryDispatcher
    {
        private readonly IQueryDispatcher inner;
        private readonly Func<IKey> userKeyGetter;

        public UserQueryDispatcher(IQueryDispatcher inner, Func<IKey> userKeyGetter)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(userKeyGetter, "userKeyGetter");
            this.inner = inner;
            this.userKeyGetter = userKeyGetter;
        }

        public Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            if (query is UserQuery userQuery)
                userQuery.UserKey = userKeyGetter();

            return inner.QueryAsync(query);
        }
    }
}
