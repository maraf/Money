using Money.Models.Queries;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    public class ListAvailableMenuItem : UserQuery, IQuery<List<IAvailableMenuItemModel>>
    {
        
    }
}
