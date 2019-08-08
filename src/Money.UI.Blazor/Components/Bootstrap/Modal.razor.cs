using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Neptuo;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components.Bootstrap
{
    public class ModalBase : ComponentBase, System.IDisposable
    {
        [Inject]
        internal ILog<ModalBase> Log { get; set; }

        [Inject]
        internal Native NativeHelper { get; set;}

        public string Id { get; private set; } = "BM" + Guid.NewGuid().ToString().Replace("-", String.Empty);

        [Parameter]
        protected string Title { get; set; }

        [Parameter]
        protected RenderFragment ChildContent { get; set; }

        [Parameter]
        protected string PrimaryButtonText { get; set; }

        /// <summary>
        /// Ivoked when user clicks on primary button.
        /// When <c>true</c> is returned, modal is closed.
        /// </summary>
        [Parameter]
        protected Func<bool> PrimaryButtonClick { get; set; }

        [Parameter]
        protected string CloseButtonText { get; set; } = "Close";

        /// <summary>
        /// Invoken when user clicks on close button.
        /// When <c>true</c> is returned, modal is closed.
        /// </summary>
        [Parameter]
        protected Func<bool> CloseButtonClick { get; set; }

        [Parameter]
        protected Action<bool> IsVisibleChanged { get; set; }

        private bool isVisible;
        private bool isVisibleChanged;

        [Parameter]
        protected bool IsVisible
        {
            get { return isVisible; }
            set
            {
                Log.Debug($"IsVisible: Current: {isVisible}, New: {value}.");
                if (isVisible != value)
                {
                    isVisible = value;

                    IsVisibleChanged?.Invoke(isVisible);
                    isVisibleChanged = true;
                }
            }
        }

        protected string DialogCssClass { get; set; }

        [Parameter]
        protected Size Size { get; set; } = Size.Normal;

        [Parameter]
        protected Action Closed { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            DialogCssClass = "modal-dialog";
            switch (Size)
            {
                case Size.Small:
                    DialogCssClass += " modal-sm";
                    break;
                case Size.Normal:
                    break;
                case Size.Large:
                    DialogCssClass += " modal-lg";
                    break;
                default:
                    throw Ensure.Exception.NotSupported(Size.ToString());
            }
        }

        protected void OnPrimaryButtonClick()
        {
            Log.Debug("Primary button click raised.");
            if (IsVisible)
            {
                Log.Debug("Visibility constraint passed.");
                IsVisible = !PrimaryButtonClick();
            }
        }

        protected void OnFormSubmit(UIEventArgs e)
        {
            Log.Debug("Form onsubmit raised.");
            if (IsVisible)
            {
                Log.Debug("Visibility constraint passed.");
                IsVisible = !PrimaryButtonClick();
            }
        }

        protected void OnCloseButtonClick()
        {
            if (CloseButtonClick != null)
                IsVisible = !CloseButtonClick();
            else
                IsVisible = false;
        }

        protected override void OnAfterRender()
        {
            base.OnAfterRender();
            NativeHelper.AddModal(Id, this);

            if (isVisibleChanged)
                NativeHelper.ToggleModal(Id, isVisible);
        }

        public void Dispose()
        {
            NativeHelper.RemoveModal(Id);
        }

        internal void MarkAsHidden()
        {
            IsVisible = false;
        }
    }
}
