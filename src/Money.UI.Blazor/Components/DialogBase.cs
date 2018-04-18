using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class DialogBase : BlazorComponent
    {
        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    IsVisibleChanged?.Invoke(isVisible);
                }
            }
        }

        public Action<bool> IsVisibleChanged { get; set; }
    }
}
