using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    public class PageNavigator : INavigator
    {
        private readonly Frame frame;
        private readonly Type pageType;

        public PageNavigator(Frame frame, Type pageType)
        {
            Ensure.NotNull(frame, "frame");
            Ensure.NotNull(pageType, "pageType");
            this.frame = frame;
            this.pageType = pageType;
        }

        public INavigatorForm Open(object parameter)
        {
            return new PageNavigatorForm(frame, pageType, parameter);
        }
    }
}
