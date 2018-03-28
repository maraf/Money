using Money.ViewModels.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.DesignData
{
    internal class MockNavigator : INavigator
    {
        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void GoForward()
        {
            throw new NotImplementedException();
        }

        public INavigatorMessageForm Message(string content)
        {
            throw new NotImplementedException();
        }

        public INavigatorMessageForm Message(string content, string title)
        {
            throw new NotImplementedException();
        }

        public INavigatorForm Open(object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
