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
        public string Title { get; set; }
        public RenderFragment ChildContent { get; set; }

        public string PrimaryButtonText { get; set; }

        /// <summary>
        /// Ivoked when user clicks on primary button.
        /// When <c>true</c> is returned, modal is closed.
        /// </summary>
        public Func<bool> PrimaryButtonClick { get; set; }

        public string CloseButtonText { get; set; } = "Close";

        /// <summary>
        /// Invoken when user clicks on close button.
        /// When <c>true</c> is returned, modal is closed.
        /// </summary>
        public Func<bool> CloseButtonClick { get; set; }

        public Action<bool> IsVisibleChanged { get; set; }
        private bool isVisible;
        public bool IsVisible
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

        public ModalSize Size { get; set; } = ModalSize.Normal;

        public Action Closed { get; set; }

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
