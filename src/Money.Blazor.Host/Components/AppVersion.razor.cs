﻿using Microsoft.AspNetCore.Components;
using Money.Events;
using Money.Queries;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class AppVersion(
        IEventHandlerCollection EventHandlers,
        IQueryDispatcher Queries
    ) : IDisposable,
        IEventHandler<ApiVersionChanged>
    {
        [Parameter]
        public bool ShowAsTable { get; set; }

        protected Version Client { get; set; }
        protected Version Api { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Client = typeof(AppVersion).Assembly.GetName().Version;
            EventHandlers.Add<ApiVersionChanged>(this);

            Api = await Queries.QueryAsync(new FindApiVersion());
        }

        public void Dispose()
        {
            EventHandlers.Remove<ApiVersionChanged>(this);
        }

        Task IEventHandler<ApiVersionChanged>.HandleAsync(ApiVersionChanged payload)
        {
            Api = payload.ApiVersion;
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}
