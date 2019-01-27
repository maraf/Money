using Neptuo;
using Neptuo.Migrations;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for migration UI.
    /// </summary>
    public class MigrateViewModel : ObservableObject, IUpgradeContext
    {
        private readonly IUpgradeService upgradeService;

        private int total;
        public int Total
        {
            get { return total; }
            private set
            {
                if (total != value)
                {
                    total = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int current;
        public int Current
        {
            get { return current; }
            private set
            {
                if (current != value)
                {
                    current = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string caption;
        public string Caption
        {
            get { return caption; }
            private set
            {
                if (caption != value)
                {
                    caption = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            private set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    RaisePropertyChanged();
                }
            }
        }

        public MigrateViewModel(IUpgradeService upgradeService)
        {
            Ensure.NotNull(upgradeService, "upgradeService");
            this.upgradeService = upgradeService;
        }

        public async Task StartAsync()
        {
            IsRunning = true;
            await upgradeService.UpgradeAsync(this);
            IsRunning = false;
        }

        IUpgradeContext IUpgradeContext.TotalSteps(int count)
        {
            Total = count;
            Caption = "Preparing.";
            return this;
        }

        IUpgradeContext IUpgradeContext.StartingStep(int index, string caption)
        {
            Current = index + 1;
            Caption = caption;
            return this;
        }
    }
}
