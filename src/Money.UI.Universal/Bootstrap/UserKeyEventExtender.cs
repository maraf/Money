using Money.Events;
using Neptuo.Collections.Specialized;
using Neptuo.Formatters;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    public class UserKeyEventExtender : ICompositeFormatterExtender
    {
        protected static class Name
        {
            public const string UserKey = "UserKey";
        }

        public void Load(IReadOnlyKeyValueCollection storage, object output)
        {
            // TODO: Not TryGet, UserKey is required!
            if (output is UserEvent payload)
            {
                if (storage.TryGet(Name.UserKey, out IKey userKey))
                    payload.UserKey = userKey;
                else
                    payload.UserKey = StringKey.Empty("User");
            }
        }

        public void Store(IKeyValueCollection storage, object input)
        {
            // TODO: Not null check, UserKey is required!
            if (input is UserEvent payload && payload.UserKey != null)
                storage.Add(Name.UserKey, payload.UserKey);
        }
    }
}
