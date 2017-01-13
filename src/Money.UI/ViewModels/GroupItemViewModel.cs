using Neptuo;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// A view model for a single item of <see cref="GroupViewModel"/>.
    /// </summary>
    public class GroupItemViewModel
    {
        /// <summary>
        /// Get a parent view model.
        /// </summary>
        public GroupViewModel Parent { get; private set; }

        /// <summary>
        /// Gets a title of the group.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets a navigation parameter of the group.
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="parent">A parent view model.</param>
        /// <param name="title">A title of the group.</param>
        /// <param name="parameter">A navigation parameter of the group.</param>
        public GroupItemViewModel(GroupViewModel parent, string title, object parameter)
        {
            Ensure.NotNull(parent, "parent");
            Parent = parent;
            Title = title;
            Parameter = parameter;
        }
    }
}
