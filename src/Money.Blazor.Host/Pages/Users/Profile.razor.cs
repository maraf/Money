using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages.Users
{
    public partial class Profile : IEventHandler<EmailChanged>, IDisposable
    {
        private ProfileModel model;

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        public string UserName { get; private set; }
        public string Email { get; set; }

        public bool IsSuccess { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            BindEvents();
            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            model = await Queries.QueryAsync(new GetProfile());
            Email = model.Email;
            UserName = model.UserName;
        }

        protected async Task OnFormSubmit()
        {
            IsSuccess = false;

            if (Email != model.Email)
                await Commands.HandleAsync(new ChangeEmail(Email));
        }

        public void Dispose()
        {
            UnBindEvents();
        }

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<EmailChanged>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<EmailChanged>(this);
        }

        async Task IEventHandler<EmailChanged>.HandleAsync(EmailChanged payload)
        {
            IsSuccess = true;
            await ReloadAsync();

            StateHasChanged();
        }

        #endregion
    }
}
