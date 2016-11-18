using Neptuo;
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
    public class NavigateCommand<T> : Command
    {
        private readonly object parameter;

        public NavigateCommand(object parameter)
        {
            this.parameter = parameter;
        }

        public NavigateCommand()
        { }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            Frame rootFrame = (Frame)Window.Current.Content;
            rootFrame.Navigate(typeof(T), parameter);
        }
    }
}
