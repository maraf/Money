using Money.ViewModels.Navigation;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    internal class PageNavigatorForm : INavigatorForm
    {
        private readonly Frame frame;
        private readonly Type pageType;
        private readonly object parameter;

        public PageNavigatorForm(Frame frame, Type pageType, object parameter)
        {
            Ensure.NotNull(frame, "frame");
            Ensure.NotNull(pageType, "pageType");
            this.frame = frame;
            this.pageType = pageType;
            this.parameter = parameter;
        }

        public virtual void Show()
        {
            frame.Navigate(pageType, parameter);
        }
    }
}
