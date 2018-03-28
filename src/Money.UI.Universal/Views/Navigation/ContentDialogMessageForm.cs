using Money.ViewModels.Navigation;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Navigation
{
    internal class ContentDialogMessageForm : INavigatorMessageForm
    {
        private readonly List<Tuple<string, ICommand>> buttons = new List<Tuple<string, ICommand>>();
        private readonly object content;
        private readonly string title;

        public ContentDialogMessageForm(object content, string title)
        {
            Ensure.NotNull(content, "content");
            this.content = content;
            this.title = title;
        }

        public INavigatorMessageForm Button(string text, ICommand action)
        {
            Ensure.NotNullOrEmpty(text, "text");
            Ensure.NotNull(action, "action");

            if (buttons.Count == 2)
                throw Ensure.Exception.NotSupported("Maximum count of buttons is 2.");

            buttons.Add(new Tuple<string, ICommand>(text, action));
            return this;
        }

        public INavigatorMessageForm ButtonClose(string text)
        {
            Ensure.NotNullOrEmpty(text, "text");
            buttons.Add(new Tuple<string, ICommand>(text, null));
            return this;
        }

        public void Show()
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Content = content;

            if (title != null)
                dialog.Title = title;

            if (buttons.Count > 0)
            {
                dialog.PrimaryButtonText = buttons[0].Item1;

                if (buttons[0].Item2 == null)
                    dialog.PrimaryButtonClick += (sender, e) => dialog.Hide();
                else
                    dialog.PrimaryButtonCommand = buttons[0].Item2;
            }
            else
            {
                dialog.PrimaryButtonText = "Close";
                dialog.PrimaryButtonClick += (sender, e) => dialog.Hide();
            }

            if (buttons.Count > 1)
            {
                dialog.SecondaryButtonText = buttons[1].Item1;
                if (buttons[1].Item2 == null)
                    dialog.SecondaryButtonClick += (sender, e) => dialog.Hide();
                else
                    dialog.SecondaryButtonCommand = buttons[1].Item2;
            }

            dialog.ShowAsync();
        }
    }
}
