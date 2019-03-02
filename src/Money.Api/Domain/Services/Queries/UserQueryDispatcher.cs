using Microsoft.AspNetCore.Http;
using Money.Models.Queries;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Queries
{
    public class UserQueryDispatcher : IQueryDispatcher
    {
        private readonly IQueryDispatcher inner;
        private readonly HttpContext httpContext;

        public UserQueryDispatcher(IQueryDispatcher inner, HttpContext httpContext)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(httpContext, "httpContext");
            this.inner = inner;
            this.httpContext = httpContext;
        }

        public Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            if (query is UserQuery userQuery)
            {
                string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!String.IsNullOrEmpty(userId))
                    userQuery.UserKey = StringKey.Create(userId, "User");
            }

            return inner.QueryAsync(query);
        }
    }
}
