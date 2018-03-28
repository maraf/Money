using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Navigation
{
    /// <summary>
    /// An opened navigation.
    /// </summary>
    public interface INavigatorForm
    {
        /// <summary>
        /// Completes the navigation to the target page.
        /// </summary>
        void Show();
    }
}
