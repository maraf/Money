using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

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

            SystemNavigationManager manater = SystemNavigationManager.GetForCurrentView();
            manater.BackRequested += OnBackRequested;

            rootFrame.Navigating += OnRootFrameNavigating;
            rootFrame.Navigated += OnRootFrameNavigated;
        }

        public void GoBack()
        {
            Template template = rootFrame.Content as Template;
            if (template != null)
                NavigateBack(template.ContentFrame);
        }

        public void GoForward()
        {
            Template template = rootFrame.Content as Template;
            if (template != null)
                NavigateForward(template.ContentFrame);
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
            {
                Summary summary = template.ContentFrame.Content as Summary;
                SummaryParameter summaryParameter = parameter as SummaryParameter;
                if (summary != null && summaryParameter != null)
                    summary.DecorateParameter(summaryParameter);

                return new PageNavigatorForm(template.ContentFrame, pageType, parameter);
            }

            throw Ensure.Exception.InvalidOperation("Missing navigation page for parameter of type '{0}'.", parameterType.FullName);
        }

        private void OnRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Template template = rootFrame.Content as Template;
            if (template != null)
            {
                template.ContentFrame.Navigated -= OnTemplateContentFrameNavigated;
                template.ContentFrame.Navigating -= OnTemplateContentFrameNavigating;
            }
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            Template template = rootFrame.Content as Template;
            if (template != null)
            {
                template.ContentFrame.Navigated += OnTemplateContentFrameNavigated;
                template.ContentFrame.Navigating += OnTemplateContentFrameNavigating;
            }
        }

        private void OnTemplateContentFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Frame frame = (Frame)sender;

            Template template = rootFrame.Content as Template;
            if (template != null && e.Parameter != null)
                template.UpdateActiveMenuItem(e.Parameter);

            Page page = frame.Content as Page;
            if (page != null)
            {
                page.PointerPressed -= OnPagePointerPressed;
                RemoveMainMenuButton(page);
            }
        }

        private void OnTemplateContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            Frame frame = (Frame)sender;
            EnsureBackButtonVisibility(frame);

            Page page = frame.Content as Page;
            if (page != null)
            {
                page.PointerPressed += OnPagePointerPressed;
                AddMainMenuButton(page);
            }
        }

        private void RemoveMainMenuButton(Page page)
        {
            if (page.BottomAppBar != null)
            {
                MainMenuAppBarToggleButton button = page.BottomAppBar.Content as MainMenuAppBarToggleButton;
                if (button != null)
                {
                    button.Dispose();
                    page.BottomAppBar.Content = null;
                }
            }
        }

        private void AddMainMenuButton(Page page)
        {
            if (page.BottomAppBar != null)
            {
                Template template = (Template)rootFrame.Content;

                MainMenuAppBarToggleButton button = new MainMenuAppBarToggleButton();
                button.SetBinding(
                    AppBarToggleButton.IsCheckedProperty,
                    new Binding()
                    {
                        Path = new PropertyPath(nameof(Template.IsMainMenuOpened)),
                        Source = template,
                        Mode = BindingMode.TwoWay
                    }
                );
                button.SetBinding(
                    AppBarToggleButton.IsCompactProperty,
                    new Binding()
                    {
                        Path = new PropertyPath(nameof(CommandBar.IsOpen)),
                        Source = page.BottomAppBar,
                        Converter = (IValueConverter)template.Resources["TrueToFalseConverter"],
                        Mode = BindingMode.OneWay
                    }
                );
                page.BottomAppBar.Content = button;
            }
        }

        private void OnPagePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                Page page = (Page)sender;
                PointerPoint point = e.GetCurrentPoint(page);
                if (point.Properties.IsXButton1Pressed)
                    NavigateBack(page.Frame);
                else if (point.Properties.IsXButton2Pressed)
                    NavigateForward(page.Frame);
            }
        }

        private void EnsureBackButtonVisibility(Frame rootFrame)
        {
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Template template = rootFrame.Content as Template;
            if (template == null)
                return;

            if (NavigateBack(template.ContentFrame))
                e.Handled = true;
        }

        private bool NavigateBack(Frame rootFrame)
        {
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack(new DrillInNavigationTransitionInfo());
                return true;
            }

            return false;
        }

        private bool NavigateForward(Frame rootFrame)
        {
            if (rootFrame.CanGoForward)
            {
                rootFrame.GoForward();
                return true;
            }

            return false;
        }

        public INavigatorMessageForm Message(string content, string title)
        {
            return new ContentDialogMessageForm(content, title);
        }

        public INavigatorMessageForm Message(string content)
        {
            return Message(content, null);
        }
    }
}
