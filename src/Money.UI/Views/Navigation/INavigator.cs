using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Navigation
{
    /// <summary>
    /// A contract for navigating between pages using parameters and associated pages.
    /// </summary>
    public interface INavigator
    {
        INavigatorForm Open(object parameter);
    }
}
