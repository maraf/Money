using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public static class UserEntityExtensions
    {
        public static bool IsUserKey(this IUserEntity entity, IKey userKey)
            => entity.UserId == userKey.AsStringKey().Identifier;

        public static T SetUserKey<T>(this T entity, IKey userKey) where T : IUserEntity
        {
            entity.UserId = userKey.AsStringKey().Identifier;
            return entity;
        }
    }
}
