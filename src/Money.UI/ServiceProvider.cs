using Money.Services;
using Money.Views.Navigation;
using Neptuo.Migrations;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Money.Services.Tiles;

namespace Money
{
    internal static class ServiceProvider
    {
        public static IQueryDispatcher QueryDispatcher { get; set; }
        public static IDomainFacade DomainFacade { get; set; }
        public static INavigator Navigator { get; set; }
        public static IUpgradeService UpgradeService { get; set; }
        public static TileService TileService { get; set; }
    }
}
