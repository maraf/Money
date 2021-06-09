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
    public partial class DateBox
    {
        [Inject]
        protected IQueryDispatcher Queries { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public bool AutoFocus { get; set; }

        [Parameter]
        public DateTime Value { get; set; }

        [Parameter]
        public Action<DateTime> ValueChanged { get; set; }

        protected string Format { get; set; }

        protected async void OnAuthenticationChanged(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                Format = await Queries.QueryAsync(new GetDateFormatProperty());
                StateHasChanged();
            }
        }

        protected void OnValueChanged(ChangeEventArgs e)
        {
            string rawValue = e.Value?.ToString();
            if (TryParseDate(rawValue, Format, out var value) || TryParseDate(rawValue, SimplifyFormat(), out value) || DateTime.TryParse(rawValue, out value))
            {
                value = TimeZoneInfo.ConvertTimeToUtc(value).Date;
                ValueChanged?.Invoke(Value = value);
            }

            string SimplifyFormat() 
                => Format.Replace("dd", "d").Replace("MM", "M");

            static bool TryParseDate(string rawValue, string format, out DateTime value) 
                => DateTime.TryParseExact(rawValue, format, null, DateTimeStyles.None, out value);
        }
    }
}
