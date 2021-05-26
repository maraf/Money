using Money.Models;
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
    /// A query providing selected bottom menu items.
    /// </summary>
    public class ListBottomMenuItem : UserQuery, IQuery<List<IActionMenuItemModel>>
    { }
}
