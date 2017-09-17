using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.Views
{
    public interface ITemplate
    {
        ResourceDictionary Resources { get; }

        Frame ContentFrame { get; }

        bool IsMainMenuOpened { get; set; }

        /// <summary>
        /// Updates currently active/selected menu item to match <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">A parameter to by selected.</param>
        void UpdateActiveMenuItem(object parameter);

        void ShowLoading();
        void HideLoading();
    }
}
