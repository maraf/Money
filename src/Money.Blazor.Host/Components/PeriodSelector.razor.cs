using Microsoft.AspNetCore.Components;
using Money.Components.Bootstrap;
using Money.Models.Loading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class PeriodSelector<T>
    {
        [Parameter]
        public T Selected { get; set; }

        [Parameter]
        public IReadOnlyCollection<T> Previous { get; set; }

        [Parameter]
        public Func<T, string> LinkFactory { get; set; }

        [Parameter]
        public Func<Task<IReadOnlyCollection<T>>> ExactGetter { get; set; }

        protected LoadingContext Loading { get; } = new LoadingContext();
        protected Modal SelectorModal { get; set; }
        protected IReadOnlyCollection<T> Periods { get; private set; }

        protected async Task LoadAsync()
        {
            using (Loading.Start())
                Periods = await ExactGetter();
        }

        protected async Task OpenSelectorAsync()
        {
            SelectorModal.Show();
            await LoadAsync();
        }
    }
}
