using Money.Data;
using Money.Services;
using Money.Services.Settings;
using Money.Services.Tiles;
using Money.ViewModels.Navigation;
using Neptuo.Activators;
using Neptuo.Events;
using Neptuo.Migrations;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    internal static class ServiceProvider
    {
        public static IQueryDispatcher QueryDispatcher { get; set; }
        public static IDomainFacade DomainFacade { get; set; }
        public static IEventHandlerCollection EventHandlers { get; set; }
        public static INavigator Navigator { get; set; }
        public static IUpgradeService UpgradeService { get; set; }
        public static TileService TileService { get; set; }
        public static IUserPreferenceService UserPreferences { get; set; }

        public static IDevelopmentService DevelopmentService { get; set; }

        internal static IFactory<EventSourcingContext> EventSourcingContextFactory { get; set; }
        internal static IFactory<ReadModelContext> ReadModelContextFactory { get; set; }
    }
}
