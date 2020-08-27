using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
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
        internal ModalInterop Interop { get; set; }

        [Inject]
        internal ILog<Modal> Log { get; set; }

        [Inject]
        internal NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string TitleIcon { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment Buttons { get; set; }

        [Parameter]
        public string CloseButtonText { get; set; } = "Close";

        [Parameter]
        public Action FormSubmit { get; set; }

        [Parameter]
        public Action CloseButtonClick { get; set; }

        [Parameter]
        public string BodyCssClass { get; set; }

        protected string DialogCssClass { get; set; }

        [Parameter]
        public Size Size { get; set; } = Size.Normal;

        [Parameter]
        public Action Closed { get; set; }

        protected ElementReference Container { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        public void Dispose()
        {
            Hide();
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e) 
            => Hide();

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

        protected void OnFormSubmit(EventArgs e) => FormSubmit?.Invoke();

        protected void OnCloseButtonClick()
        {
            Log.Debug("OnCloseButtonClick");

            if (CloseButtonClick != null)
                CloseButtonClick();
            else
                Hide();
        }

        public void Show() 
            => Interop.Show(Container);

        public void Hide() 
            => Interop.Hide(Container);
    }
}
