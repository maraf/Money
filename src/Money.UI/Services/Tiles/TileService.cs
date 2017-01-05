using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.StartScreen;

namespace Money.Services.Tiles
{
    public class TileService
    {
        public const string OutcomeCreate = "OutcomeCreate";

        public bool TryParseNavigation(LaunchActivatedEventArgs e, out object parameter)
        {
            if (e.TileId == OutcomeCreate)
            {
                if (e.Arguments.Contains("CategoryKey="))
                {
                    string rawGuid = e.Arguments.Substring(e.Arguments.LastIndexOf('=') + 1);
                    Guid guid;
                    if (Guid.TryParse(rawGuid, out guid))
                    {
                        parameter = new OutcomeParameter(GuidKey.Create(guid, KeyFactory.Empty(typeof(Category)).Type));
                    }
                    else
                    {
                        parameter = null;
                        return false;
                    }
                }
                else
                {
                    parameter = new OutcomeParameter();
                }

                return true;
            }

            parameter = null;
            return false;
        }

        public async Task PinOutcomeCreate(IKey categoryKey)
        {
            if (SecondaryTile.Exists(OutcomeCreate))
                return;

            SecondaryTile tile = new SecondaryTile(
                "OutcomeCreate",
                "Create Outcome",
                "OutcomeCreate" + (!categoryKey.IsEmpty ? "&CategoryKey=" + categoryKey.AsGuidKey().Guid : String.Empty),
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"),
                TileSize.Default
            );
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.RoamingEnabled = false;

            bool isPinned = await tile.RequestCreateAsync();
        }
    }
}
