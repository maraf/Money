using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Navigation
{
    /// <summary>
    /// A contract for navigating between pages using parameters and associated pages.
    /// </summary>
    public interface INavigator
    {
        /// <summary>
        /// Starts a navigation using <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The parameter to determine target page and pass it in.</param>
        /// <returns>An open navigation.</returns>
        INavigatorForm Open(object parameter);

        /// <summary>
        /// Navigates back, if it is possible.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Navigates forward, if it is possible.
        /// </summary>
        void GoForward();

        /// <summary>
        /// Shows a notification to user.
        /// </summary>
        /// <param name="content">A text of the notification.</param>
        INavigatorMessageForm Message(string content);

        /// <summary>
        /// Shows a notification to user.
        /// </summary>
        /// <param name="content">A text of the notification.</param>
        /// <param name="title">A title of the notification.</param>
        INavigatorMessageForm Message(string content, string title);
    }
}
