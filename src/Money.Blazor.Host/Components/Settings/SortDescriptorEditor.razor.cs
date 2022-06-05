using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Models;
using Money.Models.Sorting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components.Settings
{
    public partial class SortDescriptorEditor<T>
    {
        protected List<(string Name, T Value)> Properties { get; set; }
        protected List<(string Name, SortDirection Value)> Directions { get; set; }
        
        [Parameter]
        public T Property { get; set; }

        [Parameter]
        public Action<T> PropertyChanged { get; set; }
        
        [Parameter]
        public SortDirection Direction { get; set; }

        [Parameter]
        public Action<SortDirection> DirectionChanged { get; set; }

        protected override void OnInitialized()
        {
            Properties = new List<(string Name, T Value)>();
            SortButton<T>.BuildItems(Properties);

            Directions = new List<(string Name, SortDirection Value)>();
            SortButton<SortDirection>.BuildItems(Directions);
        }

        protected void OnPropertyChanged(ChangeEventArgs e) 
            => ParseAndRaise<T>(e, PropertyChanged);

        protected void OnDirectionChanged(ChangeEventArgs e) 
            => ParseAndRaise<SortDirection>(e, DirectionChanged);

        private void ParseAndRaise<TEnum>(ChangeEventArgs e, Action<TEnum> handler) 
            where TEnum : struct
        {
            string rawValue = e.Value?.ToString();
            if (Enum.TryParse(rawValue, out TEnum value))
                handler?.Invoke(value);
        }
    }
}