using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Controls
{
    public class SelectedItemEventArgs :EventArgs
    {
        public object SelectedItem { get; private set; }

        public SelectedItemEventArgs(object selectedItem)
        {
            SelectedItem = selectedItem;
        }
    }
}
