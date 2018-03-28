using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Money.Views.Dialogs;
using Money.Views.StateTriggers;
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
    internal class AppNavigator : INavigator
    {
        private readonly NavigatorParameterCollection rules;
        private readonly Frame rootFrame;

        public AppNavigator(NavigatorParameterCollection rules, Frame rootFrame)
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
            bool isOutcome;

            ITemplate template = rootFrame.Content as ITemplate;
            if (template != null)
            {
                if (NavigateBack(template.ContentFrame))
                    return;

                isOutcome = template.ContentFrame.Content is OutcomeCreate;
            }
            else
            {
                isOutcome = rootFrame.Content is OutcomeCreate;
            }

            if (isOutcome)
            {
                OutcomeCreatedGuidePost dialog = new OutcomeCreatedGuidePost();
                dialog.ShowAsync();
            }
        }

        public void GoForward()
        {
            ITemplate template = rootFrame.Content as ITemplate;
            if (template != null)
                NavigateForward(template.ContentFrame);
        }

        public INavigatorForm Open(object parameter)
        {
            Ensure.NotNull(parameter, "parameter");

            Type pageType;
            Type parameterType = parameter.GetType();

            ITemplate template = rootFrame.Content as ITemplate;
            if (template == null)
            {
                if (parameterType == typeof(MigrateParameter) && rules.TryGetPageType(parameterType, out pageType))
                    return new PageNavigatorForm(rootFrame, pageType, parameter);

                Type templateType = typeof(Template);
                if (new MobileStateTrigger().IsActive)
                    templateType = typeof(TemplateMobile);

                return new PageNavigatorForm(rootFrame, templateType, parameter);
            }

            if (rules.TryGetPageType(parameterType, out pageType))
            {
                Summary summary = template.ContentFrame.Content as Summary;
                SummaryParameter summaryParameter = parameter as SummaryParameter;
                if (summary != null && summaryParameter != null)
                    summary.DecorateParameter(summaryParameter);

                return new PageNavigatorForm(template.ContentFrame, pageType, parameter);
            }

            if (rules.TryGetWizardType(parameterType, out pageType))
                return new WizardNavigatorForm(pageType, parameter, rootFrame, currentParameter);

            throw Ensure.Exception.InvalidOperation("Missing navigation page for parameter of type '{0}'.", parameterType.FullName);
        }

        private void OnRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            ITemplate template = rootFrame.Content as ITemplate;
            if (template != null)
            {
                template.ContentFrame.Navigated -= OnTemplateContentFrameNavigated;
                template.ContentFrame.Navigating -= OnTemplateContentFrameNavigating;
                template.PointerPressed -= OnTemplatePointerPressed;
            }
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            ITemplate template = rootFrame.Content as ITemplate;
            if (template != null)
            {
                template.ContentFrame.Navigated += OnTemplateContentFrameNavigated;
                template.ContentFrame.Navigating += OnTemplateContentFrameNavigating;
                template.PointerPressed += OnTemplatePointerPressed;
            }
        }

        private object currentParameter;
        private object lastParameter;

        private void OnTemplatePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ITemplate template = rootFrame.Content as ITemplate;
            if (template != null)
            {
                PointerPoint point = e.GetCurrentPoint((UIElement)template);
                if (point.Properties.IsXButton1Pressed && template.IsMainMenuOpened)
                    template.IsMainMenuOpened = false;
            }
        }

        private void OnTemplateContentFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Frame frame = (Frame)sender;
            currentParameter = e.Parameter;

            ITemplate template = rootFrame.Content as ITemplate;
            if (template != null)
            {
                if (e.Parameter != null)
                    template.UpdateActiveMenuItem(e.Parameter);
            }

            Page page = frame.Content as Page;
            if (page != null)
            {
                page.PointerPressed -= OnPagePointerPressed;
                RemoveMainMenuButton(page);
            }

            INavigatorParameterPage parameterPage = frame.Content as INavigatorParameterPage;
            if (parameterPage == null)
                lastParameter = null;
            else
                lastParameter = parameterPage.Parameter;
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

            ITemplate template = rootFrame.Content as ITemplate;
            INavigatorPage navigatorPage = frame.Content as INavigatorPage;
            if (navigatorPage != null && template != null)
            {
                template.ShowLoading();
                navigatorPage.ContentLoaded += OnTemplateContentPageLoaded;
            }

            if (lastParameter != null)
            {
                PageStackEntry lastEntry = template.ContentFrame.BackStack.LastOrDefault();
                if (lastEntry != null)
                {
                    template.ContentFrame.BackStack.Remove(lastEntry);
                    lastEntry = new PageStackEntry(
                        lastEntry.SourcePageType,
                        lastParameter,
                        lastEntry.NavigationTransitionInfo
                    );
                    template.ContentFrame.BackStack.Add(lastEntry);
                }
            }
        }

        private void OnTemplateContentPageLoaded(object sender, EventArgs e)
        {
            INavigatorPage navigatorPage = sender as INavigatorPage;
            if (navigatorPage != null)
                navigatorPage.ContentLoaded -= OnTemplateContentPageLoaded;

            ITemplate template = rootFrame.Content as ITemplate;
            if (template != null)
                template.HideLoading();
        }

        private void RemoveMainMenuButton(Page page)
        {
            if (page.BottomAppBar != null)
            {
                MainMenuAppBarButton button = page.BottomAppBar.Content as MainMenuAppBarButton;
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
                ITemplate template = (ITemplate)rootFrame.Content;

                MainMenuAppBarButton button = new MainMenuAppBarButton();
                button.SetBinding(
                    AppBarToggleButton.IsCheckedProperty,
                    new Binding()
                    {
                        Path = new PropertyPath(nameof(ITemplate.IsMainMenuOpened)),
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
                        Converter = (IValueConverter)Application.Current.Resources["TrueToFalseConverter"],
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

                Frame frame = null;
                ITemplate template = rootFrame.Content as ITemplate;
                if (template == null)
                    frame = page.Frame;
                else
                    frame = template.ContentFrame;

                PointerPoint point = e.GetCurrentPoint(page);
                if (point.Properties.IsXButton1Pressed)
                    NavigateBack(frame);
                else if (point.Properties.IsXButton2Pressed)
                    NavigateForward(frame);
            }
        }

        private void EnsureBackButtonVisibility(Frame rootFrame)
        {
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            ITemplate template = rootFrame.Content as ITemplate;
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
