using Money.Data;
using Money.Services;
using Money.Services.Globalization;
using Money.Services.Settings;
using Money.Services.Tiles;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Neptuo.Activators;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Migrations;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money
{
    internal static class ServiceProvider
    {
        public static MessageBuilder MessageBuilder { get; set; }
        public static IQueryDispatcher QueryDispatcher { get; set; }
        public static ICommandDispatcher CommandDispatcher { get; set; }
        public static IEventHandlerCollection EventHandlers { get; set; }
        public static INavigator Navigator { get; set; }
        public static IUpgradeService UpgradeService { get; set; }
        public static TileService TileService { get; set; }
        public static IUserPreferenceService UserPreferences { get; set; }
        public static IPriceConverter PriceConverter { get; set; }
        public static ICurrencyProvider CurrencyProvider { get; set; }

        public static IDevelopmentService DevelopmentTools { get; set; }
        public static RestartService RestartService { get; set; }

        internal static AppDataService AppDataService { get; set; }

        internal static IFactory<EventSourcingContext> EventSourcingContextFactory { get; set; }
        internal static IFactory<ReadModelContext> ReadModelContextFactory { get; set; }
        internal static IFactory<ApplicationDataContainer> StorageContainerFactory { get; set; }

        public static IFactory<IReadOnlyList<MenuItemViewModel>, bool> MainMenuFactory { get; set; }
    }
}
