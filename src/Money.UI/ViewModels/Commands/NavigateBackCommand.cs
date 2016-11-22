using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.ViewModels.Commands
{
    public class NavigateBackCommand : Command
    {
        public override bool CanExecute()
        {
            Frame rootFrame = (Frame)Window.Current.Content;
            return rootFrame.CanGoBack;
        }

        public override void Execute()
        {
            Frame rootFrame = (Frame)Window.Current.Content;
            rootFrame.GoBack();
        }
    }
}
