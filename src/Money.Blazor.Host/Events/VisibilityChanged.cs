using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    public class VisibilityChanged : Event
    {
        public bool IsVisible { get; }

        public VisibilityChanged(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}
