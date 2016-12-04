using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    /// <summary>
    /// Contains a stack of frame which orchestrates composite navigation history.
    /// </summary>
    public class FrameBackStack
    {
        private readonly List<Frame> state = new List<Frame>();
        private int index;

        public FrameBackStack Add(Frame frame)
        {
            throw new NotImplementedException();
        }

        public void GoBack()
        {
            throw new NotImplementedException();
            // TODO: Find last frame and try to go back.
        }

        public void GoForward()
        {
            throw new NotImplementedException();
            // TODO: Find last frame and try to go forward.
        }
    }
}
