using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    public abstract class UserQuery
    {
        private IKey userKey = StringKey.Empty("User");

        public IKey UserKey
        {
            get { return userKey; }
            set
            {
                if (value == null)
                    userKey = StringKey.Empty("User");
                else
                    userKey = value;
            }
        }
    }
}
