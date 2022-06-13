using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class ExpenseTemplates : IDisposable, IEventHandler<ExpenseTemplateCreated>, IEventHandler<ExpenseTemplateDeleted>
    {
        [Inject]
        protected IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        protected ExpenseTemplateCreate Modal { get; set; }
        protected List<ExpenseTemplateModel> Models { get; } = new List<ExpenseTemplateModel>();

        protected async override Task OnInitializedAsync()
        {
            EventHandlers
                .Add<ExpenseTemplateCreated>(this)
                .Add<ExpenseTemplateDeleted>(this);

            await base.OnInitializedAsync();

            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {
            Models.Clear();
            Models.AddRange(await Queries.QueryAsync(new ListAllExpenseTemplate()));
            StateHasChanged();
        }

        public void Dispose() => EventHandlers
                .Remove<ExpenseTemplateCreated>(this)
                .Remove<ExpenseTemplateDeleted>(this);

        public Task HandleAsync(ExpenseTemplateCreated payload)
        {
            _ = ReloadAsync();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ExpenseTemplateDeleted payload)
        {
            _ = ReloadAsync();
            return Task.CompletedTask;
        }
    }
}