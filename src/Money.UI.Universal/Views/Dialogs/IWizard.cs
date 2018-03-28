using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Dialogs
{
    public interface IWizard
    {
        Task ShowAsync(object parameter);
    }
}
