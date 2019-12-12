using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public class DialogBase : ComponentBase
    {
        private bool isVisible;

        [Parameter]
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

        [Parameter]
        public Action<bool> IsVisibleChanged { get; set; }
    }
}
