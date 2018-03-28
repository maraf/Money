using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Money.ViewModels.Navigation
{
    public interface INavigatorMessageForm : INavigatorForm
    {
        INavigatorMessageForm Button(string text, ICommand action);
        INavigatorMessageForm ButtonClose(string text);
    }
}
