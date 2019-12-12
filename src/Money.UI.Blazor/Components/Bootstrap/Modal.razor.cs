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
    public partial class Modal : System.IDisposable
    {
        [Inject]
        internal ILog<Modal> Log { get; set; }

        [Inject]
        internal Native NativeHelper { get; set;}

        public string Id { get; private set; } = "BM" + Guid.NewGuid().ToString().Replace("-", String.Empty);

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string PrimaryButtonText { get; set; }

        /// <summary>
        /// Ivoked when user clicks on primary button.
        /// When <c>true</c> is returned, modal is closed.
        /// </summary>
        [Parameter]
        public Func<bool> PrimaryButtonClick { get; set; }

        [Parameter]
        public string CloseButtonText { get; set; } = "Close";

        /// <summary>
        /// Invoken when user clicks on close button.
        /// When <c>true</c> is returned, modal is closed.
        /// </summary>
        [Parameter]
        public Func<bool> CloseButtonClick { get; set; }

        [Parameter]
        public Action<bool> IsVisibleChanged { get; set; }

        private bool isVisible;
        private bool isVisibleChanged;

        [Parameter]
        public bool IsVisible
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
        public Size Size { get; set; } = Size.Normal;

        [Parameter]
        public Action Closed { get; set; }

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

        protected void OnFormSubmit(EventArgs e)
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

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
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
