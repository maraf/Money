using Money.ViewModels.Parameters;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.StartScreen;

namespace Money.Services.Tiles
{
    public class TileService
    {
        public const string OutcomeCreatePrefix = "OutcomeCreate-";
        public const string OutcomeCreateFormat = "OutcomeCreate-{0}";

        public bool TryParseNavigation(LaunchActivatedEventArgs e, out object parameter)
        {
            if (e.TileId.StartsWith(OutcomeCreatePrefix))
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

        public async Task PinOutcomeCreate(IKey categoryKey, string categoryName, Color? backgroundColor)
        {
            if (SecondaryTile.Exists(OutcomeCreateFormat))
                return;

            string guid = null;
            if (!categoryKey.IsEmpty)
                guid = categoryKey.AsGuidKey().Guid.ToString();

            string tileId = String.Format(OutcomeCreateFormat, guid ?? "Empty");

            string displayName = "Create Outcome";
            if (!categoryKey.IsEmpty)
                displayName += $" in '{categoryName}'";

            SecondaryTile tile = new SecondaryTile(
                tileId,
                displayName,
                "OutcomeCreate" + (!categoryKey.IsEmpty ? "&CategoryKey=" + guid : String.Empty),
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"),
                TileSize.Square150x150
            );
            tile.VisualElements.Square71x71Logo = new Uri("ms-appx:///Assets/Square71x71Logo.scale-200.png");
            tile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.RoamingEnabled = false;

            if (backgroundColor != null)
                tile.VisualElements.BackgroundColor = ColorConverter.Map(backgroundColor.Value);

            await tile.RequestCreateAsync();
        }
    }
}
