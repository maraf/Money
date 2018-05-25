using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Browser.Interop;
using Microsoft.AspNetCore.Blazor.Components;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components.Bootstrap
{
    public class ModalBase : BlazorComponent, System.IDisposable
    {
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

        [Parameter]
        protected bool IsVisible
        {
            get { return isVisible; }
            set
            {
                Console.WriteLine($"Modal: IsVisible: Current: {isVisible}, New: {value}.");
                if (isVisible != value)
                {
                    isVisible = value;
                    RegisteredFunction.Invoke<object>("Bootstrap_Modal_Toggle", Id, isVisible);

                    IsVisibleChanged?.Invoke(isVisible);
                }
            }
        }

        protected string DialogCssClass { get; set; }

        [Parameter]
        protected ModalSize Size { get; set; } = ModalSize.Normal;

        [Parameter]
        protected Action Closed { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            DialogCssClass = "modal-dialog";
            switch (Size)
            {
                case ModalSize.Small:
                    DialogCssClass += " modal-sm";
                    break;
                case ModalSize.Normal:
                    break;
                case ModalSize.Large:
                    DialogCssClass += " modal-lg";
                    break;
                default:
                    throw Ensure.Exception.NotSupported(Size.ToString());
            }
        }

        protected void OnPrimaryButtonClick()
        {
            IsVisible = !PrimaryButtonClick();
        }

        protected void OnCloseButtonClick()
        {
            if (CloseButtonClick != null)
                IsVisible = !CloseButtonClick();
            else
                IsVisible = false;
        }

        protected override void OnInit()
        {
            base.OnInit();
            Native.AddModal(Id, this);
        }

        public void Dispose()
        {
            Native.RemoveModal(Id);
        }
    }
}
