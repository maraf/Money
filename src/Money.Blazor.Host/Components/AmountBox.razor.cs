using Microsoft.AspNetCore.Components;
using Money.Queries;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class AmountBox
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public bool AutoFocus { get; set; }

        [Parameter]
        public bool AutoSelect { get; set; }

        [Parameter]
        public decimal Value { get; set; }

        [Parameter]
        public Action<decimal> ValueChanged { get; set; }

        protected int DecimalDigits { get; set; }

        protected async void OnAuthenticationChanged(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                DecimalDigits = await Queries.QueryAsync(new GetPriceDecimalDigitsProperty());
                StateHasChanged();
            }
        }

        protected void OnValueChanged(ChangeEventArgs e)
        {
            string rawValue = e.Value?.ToString();
            if (Decimal.TryParse(rawValue, out var value))
                ValueChanged?.Invoke(Value = value);
        }
    }
}
