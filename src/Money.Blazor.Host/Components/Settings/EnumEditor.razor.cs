using Microsoft.AspNetCore.Components;
using Money.Models.Sorting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components.Settings
{
    public partial class EnumEditor<T>
    {
        protected List<(string Name, T Value)> Properties { get; set; }

        [Parameter]
        public T Property { get; set; }

        [Parameter]
        public Action<T> PropertyChanged { get; set; }

        protected override void OnInitialized()
        {
            Properties = new List<(string Name, T Value)>();
            SortButton<T>.BuildItems(Properties);
        }
    }
}
