﻿using Microsoft.AspNetCore.Components;
using Money.Queries;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public partial class DateBox(ILog<DateBox> Log, IQueryDispatcher Queries)
{
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
            RaiseValueChanged(value);

        string SimplifyFormat() 
            => Format.Replace("dd", "d").Replace("MM", "M");

        static bool TryParseDate(string rawValue, string format, out DateTime value) 
            => DateTime.TryParseExact(rawValue, format, null, DateTimeStyles.None, out value);
    }

    protected void RaiseValueChanged(DateTime value)
    {
        Log.Debug($"RaiseValueChanged source value '{value}'");
        Value = value = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, 0, DateTimeKind.Utc);
        Log.Debug($"RaiseValueChanged processed value '{value}'");
        ValueChanged?.Invoke(Value = value);
    }
}
