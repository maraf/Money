using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Money.Views.Controls
{
    public class MainMenuListView : ListView
    {
        private SplitView splitView;

        public MainMenuListView()
        {
            SelectionMode = ListViewSelectionMode.Single;
            SingleSelectionFollowsFocus = false;
            IsItemClickEnabled = true;
            ItemClick += OnItemClicked;

            Loaded += (s, a) =>
            {
                var parent = VisualTreeHelper.GetParent(this);
                while (parent != null && !(parent is SplitView))
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent != null && parent is SplitView sv)
                {
                    splitView = sv;
                    splitView.RegisterPropertyChangedCallback(SplitView.IsPaneOpenProperty, (sender, args) => OnPaneToggled());
                    splitView.RegisterPropertyChangedCallback(SplitView.DisplayModeProperty, (sender, args) => OnPaneToggled());
                    OnPaneToggled();
                }
            };
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Remove the entrance animation on the item containers.
            for (int i = 0; i < ItemContainerTransitions.Count; i++)
            {
                if (ItemContainerTransitions[i] is EntranceThemeTransition)
                    ItemContainerTransitions.RemoveAt(i);
            }
        }

        /// <summary>
        /// Mark the <paramref name="item"/> as selected and ensures everything else is not.
        /// If the <paramref name="item"/> is null then everything is unselected.
        /// </summary>
        /// <param name="item"></param>
        public void SetSelectedItem(ListViewItem item)
        {
            int index = -1;
            if (item != null)
                index = IndexFromContainer(item);

            for (int i = 0; i < Items.Count; i++)
            {
                ListViewItem container = (ListViewItem)ContainerFromIndex(i);
                container.IsSelected = i == index;
            }
        }

        /// <summary>
        /// Occurs when an item has been selected
        /// </summary>
        public event EventHandler<ListViewItem> ItemInvoked;

        /// <summary>
        /// Custom keyboarding logic to enable movement via the arrow keys without triggering selection 
        /// until a 'Space' or 'Enter' key is pressed. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            object focusedItem = FocusManager.GetFocusedElement();
            switch (e.Key)
            {
                case VirtualKey.Up:
                    TryMoveFocus(FocusNavigationDirection.Up);
                    e.Handled = true;
                    break;

                case VirtualKey.Down:
                    TryMoveFocus(FocusNavigationDirection.Down);
                    e.Handled = true;
                    break;

                case VirtualKey.Space:
                case VirtualKey.Enter:
                    // Fire our event using the item with current keyboard focus
                    InvokeItem(focusedItem);
                    e.Handled = true;
                    break;

                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        /// <summary>
        /// This method is a work-around until the bug in FocusManager.TryMoveFocus is fixed.
        /// </summary>
        /// <param name="direction"></param>
        private void TryMoveFocus(FocusNavigationDirection direction)
        {
            if (direction == FocusNavigationDirection.Next || direction == FocusNavigationDirection.Previous)
            {
                FocusManager.TryMoveFocus(direction);
            }
            else
            {
                Control control = FocusManager.FindNextFocusableElement(direction) as Control;
                if (control != null)
                    control.Focus(FocusState.Programmatic);
            }
        }

        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            // Triggered when the item is selected using something other than a keyboard
            var item = ContainerFromItem(e.ClickedItem);
            InvokeItem(item);
        }

        private void InvokeItem(object focusedItem)
        {
            SetSelectedItem(focusedItem as ListViewItem);
            ItemInvoked?.Invoke(this, focusedItem as ListViewItem);

            if (splitView != null &&
                splitView.IsPaneOpen && (
                splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay ||
                splitView.DisplayMode == SplitViewDisplayMode.Overlay))
            {
                splitView.IsPaneOpen = false;
            }

            ListViewItem listViewItem = focusedItem as ListViewItem;
            if (listViewItem != null)
                listViewItem.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Re-size the ListView's Panel when the SplitView is compact so the items
        /// will fit within the visible space and correctly display a keyboard focus rect.
        /// </summary>
        private void OnPaneToggled()
        {
            if (splitView != null)
            {
                if (splitView.IsPaneOpen)
                {
                    ItemsPanelRoot.ClearValue(FrameworkElement.WidthProperty);
                    ItemsPanelRoot.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
                }
                else if (splitView.DisplayMode == SplitViewDisplayMode.CompactInline || splitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                {
                    ItemsPanelRoot.SetValue(FrameworkElement.WidthProperty, this.splitView.CompactPaneLength);
                    ItemsPanelRoot.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                }
            }
        }
    }
}
