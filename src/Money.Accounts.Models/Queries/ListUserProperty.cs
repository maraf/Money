using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// Gets a list of all user properties.
    /// Result contains all system defined properties, even those without value for specific used.
    /// </summary>
    public class ListUserProperty : UserQuery, IQuery<List<UserPropertyModel>>
    { }
}
