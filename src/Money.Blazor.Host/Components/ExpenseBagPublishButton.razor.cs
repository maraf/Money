using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Events;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExpenseBagPublishButton : IDisposable,
        IEventHandler<LocallyStoredExpenseCreated>,
        IEventHandler<LocallyStoredExpensesPublished>
    {
        [Inject]
        internal IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        internal CreateExpenseStorage Storage { get; set; }

        [Inject]
        internal LocalExpenseOnlineRunner Runner { get; set; }

        [Parameter]
        public string Text { get; set; } = "You are online, upload your expenses";

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }

        protected bool HasLocalExpenses { get; private set; }

        protected async override Task OnInitializedAsync()
        {
            EventHandlers.Add<LocallyStoredExpenseCreated>(this);
            EventHandlers.Add<LocallyStoredExpensesPublished>(this);

            await base.OnInitializedAsync();
            await LoadAsync();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Attributes == null)
                Attributes = new Dictionary<string, object>();

            SetCssClass();
        }

        private void SetCssClass()
        {
            const string defaultCssClass = "btn btn-secondary text-truncate mw-100";

            object cssClass;
            if (Attributes.TryGetValue("class", out cssClass))
                cssClass = defaultCssClass + " " + cssClass;
            else
                cssClass = defaultCssClass;

            Attributes["class"] = cssClass;
        }

        public void Dispose()
        {
            EventHandlers.Remove<LocallyStoredExpenseCreated>(this);
            EventHandlers.Remove<LocallyStoredExpensesPublished>(this);
        }

        private async Task LoadAsync()
        {
            var items = await Storage.LoadAsync();

            HasLocalExpenses = items != null && items.Count > 0;
        }

        protected async Task PublishAsync()
        {
            await Runner.PublishAsync();
        }

        async Task IEventHandler<LocallyStoredExpenseCreated>.HandleAsync(LocallyStoredExpenseCreated payload)
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
