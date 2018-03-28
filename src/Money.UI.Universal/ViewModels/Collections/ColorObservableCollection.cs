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
    public class ColorObservableCollection : ObservableCollection<Color>
    {
        public ColorObservableCollection()
        {
            if (DesignMode.DesignModeEnabled)
            {
                Add(ColorConverter.Map(Colors.Black));
                Add(ColorConverter.Map(Colors.White));
                Add(ColorConverter.Map(Colors.Green));
                Add(ColorConverter.Map(Colors.Blue));
                Add(ColorConverter.Map(Colors.Yellow));
                Add(ColorConverter.Map(Colors.Brown));
                Add(ColorConverter.Map(Colors.RoyalBlue));
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
                if (propertyInfo.PropertyType == typeof(Color))
                    Add((Color)propertyInfo.GetValue(null));
            }
        }
    }
}
