using Money.ViewModels;
using Money.ViewModels.Parameters;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    internal class ApplicationNavigator : INavigator
    {
        private readonly NavigatorParameterCollection rules;
        private readonly Frame rootFrame;

        public ApplicationNavigator(NavigatorParameterCollection rules, Frame rootFrame)
        {
            Ensure.NotNull(rules, "rules");
            Ensure.NotNull(rootFrame, "rootFrame");
            this.rules = rules;
            this.rootFrame = rootFrame;

            SystemNavigationManager manager = SystemNavigationManager.GetForCurrentView();
            manager.BackRequested += OnBackRequested;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void GoForward()
        {
            throw new NotImplementedException();
        }

        public INavigatorForm Open(object parameter)
        {
            Ensure.NotNull(parameter, "parameter");

            Template template = rootFrame.Content as Template;
            if (template == null)
                return new PageNavigatorForm(rootFrame, typeof(Template), parameter);
            
            Type pageType;
            Type parameterType = parameter.GetType();
            if (rules.TryGetPageType(parameterType, out pageType))
                return new PageNavigatorForm(template.ContentFrame, pageType, parameter);

            throw Ensure.Exception.InvalidOperation("Missing navigation page for parameter of type '{0}'.", parameterType.FullName);
        }
    }
}
