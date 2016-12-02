using Money.ViewModels;
using Money.ViewModels.Parameters;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    /// <summary>
    /// An implementation of <see cref="INavigator"/> for <see cref="GroupPage"/>.
    /// </summary>
    internal class GroupNavigator : INavigator
    {
        private readonly INavigator inner;
        private readonly Frame frame;

        public GroupNavigator(INavigator inner, Frame frame)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(frame, "frame");
            this.inner = inner;
            this.frame = frame;
        }

        public INavigatorForm Open(object parameter)
        {
            Ensure.NotNull(parameter, "parameter");

            IGroupParameter groupParameter = parameter as IGroupParameter;
            if (groupParameter != null)
            {
                if (groupParameter.Month != null)
                    return new PageNavigatorForm(frame, typeof(GroupPage), new GroupParameter(GroupType.Month, parameter));
                else if (groupParameter.Year != null)
                    return new PageNavigatorForm(frame, typeof(GroupPage), new GroupParameter(GroupType.Year, parameter));
                else
                    throw Ensure.Exception.InvalidOperation("Missing year or month.");
            }

            return inner.Open(parameter);
        }
    }
}
