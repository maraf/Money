using Money.Models.Queries;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereUserKey<T>(this IQueryable<T> sql, UserQuery query) where T : IUserEntity
            => WhereUserKey(sql, query.UserKey);

        public static IQueryable<T> WhereUserKey<T>(this IQueryable<T> query, IKey userKey) where T : IUserEntity
            => query.Where(e => e.UserId == userKey.AsStringKey().Identifier);

        public static IQueryable<T> TakePage<T>(this IQueryable<T> query, int pageIndex, int pageSize)
            => query.Skip(pageIndex * pageSize).Take(pageSize);
    }
}
