using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Money.Views.Navigation
{
    public interface INavigatorMessageForm : INavigatorForm
    {
        INavigatorMessageForm Button(string text, ICommand action);
    }
}
