using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models.Loading;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class ExpenseBag : IDisposable, IEventHandler<CreateExpenseStoredLocally>, IEventHandler<LocallyStoredExpensesPublished>
    {
        [Inject]
        internal IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        internal CreateExpenseStorage Storage { get; set; }

        [Inject]
        internal LocalExpenseOnlineRunner Runner { get; set; }

        protected List<CreateOutcome> Models { get; } = new List<CreateOutcome>();
        protected LoadingContext Loading { get; } = new LoadingContext();

        protected ModalDialog CreateModal { get; set; }

        protected async override Task OnInitializedAsync()
        {
            EventHandlers.Add<CreateExpenseStoredLocally>(this);
            EventHandlers.Add<LocallyStoredExpensesPublished>(this);

            await base.OnInitializedAsync();
            await LoadAsync();
        }

        public void Dispose()
        {
            EventHandlers.Remove<CreateExpenseStoredLocally>(this);
            EventHandlers.Remove<LocallyStoredExpensesPublished>(this);
        }

        private async Task LoadAsync()
        {
            using (Loading.Start())
            {
                Models.Clear();
                var models = await Storage.LoadAsync();
                if (models != null)
                    Models.AddRange(models); 
            }
        }

        protected void CreateNewExpense()
        {
            CreateModal.Show();
            StateHasChanged();
        }

        protected async Task PublishAsync()
        {
            await Runner.PublishAsync();
        }

        async Task IEventHandler<CreateExpenseStoredLocally>.HandleAsync(CreateExpenseStoredLocally payload)
        {
            await LoadAsync();
            StateHasChanged();
        }

        async Task IEventHandler<LocallyStoredExpensesPublished>.HandleAsync(LocallyStoredExpensesPublished payload)
        {
            await LoadAsync();
            StateHasChanged();
        }
    }
}
