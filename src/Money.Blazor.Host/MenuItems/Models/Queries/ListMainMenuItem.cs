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
    /// A query providing main menu items.
    /// </summary>
    public class ListMainMenuItem : UserQuery, IQuery<MainMenuItems>
    { }

    public record MainMenuItems(List<MenuItemModel> Views, List<MenuItemModel> More, List<MenuItemModel> User);
}
