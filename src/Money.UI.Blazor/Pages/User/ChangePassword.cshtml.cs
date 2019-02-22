using Microsoft.AspNetCore.Blazor.Components;
using Money.Events;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public class ChangePasswordBase : BlazorComponent, IEventHandler<PasswordChanged>, IDisposable
    {
        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        public string Current { get; set; }
        public string New { get; set; }
        public string Confirm { get; set; }

        public bool IsSuccess { get; set; }

        protected override Task OnInitAsync()
        {
            BindEvents();
            return base.OnInitAsync();
        }

        protected async Task OnFormSubmit()
        {
            IsSuccess = false;

            if (!String.IsNullOrEmpty(Current) && !String.IsNullOrEmpty(New) && New == Confirm)
                await Commands.HandleAsync(new Commands.ChangePassword(Current, New));
        }

        public void Dispose()
        {
            UnBindEvents();
        }

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<PasswordChanged>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<PasswordChanged>(this);
        }

        Task IEventHandler<PasswordChanged>.HandleAsync(PasswordChanged payload)
        {
            Current = null;
            New = null;
            Confirm = null;
            IsSuccess = true;
            StateHasChanged();
            return Task.CompletedTask;
        }

        #endregion
    }
}
