using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    internal class AppNavigatorForm : PageNavigatorForm
    {
        private readonly ITemplate template;

        public AppNavigatorForm(ITemplate template, Type pageType, object parameter)
            : base(template.ContentFrame, pageType, parameter)
        {
            this.template = template;
        }

        public override void Show()
        {
            base.Show();
            template.ShowLoading();
        }
    }
}
