using Microsoft.AspNetCore.Components;
using Money.Components.Bootstrap;
using Money.Models.Sorting;
using Neptuo.Logging;
using Neptuo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class EnumSelector<TType> : ComponentBase
    {
        [Inject]
        internal ILog<SortButton<TType>> Log { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public string IconPrefix { get; set; }

        [Parameter]
        public TType Current { get; set; }

        [Parameter]
        public Action<TType> CurrentChanged { get; set; }

        [Parameter]
        public Action Changed { get; set; }

        [Parameter]
        public Size Size { get; set; } = Size.Normal;

        protected List<(string Name, TType Value)> Items { get; } = new List<(string, TType)>();
        protected string ButtonCssClass { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SortButton<TType>.BuildItems(Items);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Current == null)
                UpdateCurrent(Items.First().Value);

            ButtonCssClass = "btn btn-light dropdown-toggle";
            switch (Size)
            {
                case Size.Small:
                    ButtonCssClass += " btn-sm";
                    break;
                case Size.Normal:
                    break;
                case Size.Large:
                    ButtonCssClass += " btn-lg";
                    break;
                default:
                    throw Ensure.Exception.NotSupported(Size.ToString());
            }
        }

        private void UpdateCurrent(TType type)
        {
            Current = type;
            CurrentChanged?.Invoke(Current);
        }

        protected void OnItemClick(TType type)
        {
            UpdateCurrent(type);
            Changed?.Invoke();
        }
    }
}
