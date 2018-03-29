using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI;

namespace Money.ViewModels.Collections
{
    public class ColorObservableCollection : ObservableCollection<Windows.UI.Color>
    {
        public ColorObservableCollection()
        {
            if (DesignMode.DesignModeEnabled)
            {
                Add(Colors.Black);
                Add(Colors.White);
                Add(Colors.Green);
                Add(Colors.Blue);
                Add(Colors.Yellow);
                Add(Colors.Brown);
                Add(Colors.RoyalBlue);
            }
            else
            {
                Load();
            }
        }

        private void Load()
        {
            foreach (PropertyInfo propertyInfo in typeof(Colors).GetRuntimeProperties())
            {
                if (propertyInfo.PropertyType == typeof(Windows.UI.Color))
                    Add((Windows.UI.Color)propertyInfo.GetValue(null));
            }
        }
    }
}
