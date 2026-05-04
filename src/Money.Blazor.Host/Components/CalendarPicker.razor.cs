using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;

namespace Money.Components;

public partial class CalendarPicker
{
    [Parameter]
    public string Id { get; set; }

    [Parameter]
    public DateTime Value { get; set; }

    [Parameter]
    public Action<DateTime> ValueChanged { get; set; }

    [Parameter]
    public bool AutoFocus { get; set; }

    protected CalendarPickerPart CurrentPart { get; set; } = CalendarPickerPart.Day;
    protected int CurrentYear { get; set; }
    protected int CurrentMonth { get; set; }

    protected string[] MonthNames => DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Value != DateTime.MinValue)
        {
            CurrentYear = Value.Year;
            CurrentMonth = Value.Month;
        }
        else
        {
            CurrentYear = AppDateTime.Today.Year;
            CurrentMonth = AppDateTime.Today.Month;
        }
    }

    protected string DayCssClass(int day)
    {
        if (Value.Year == CurrentYear && Value.Month == CurrentMonth && Value.Day == day)
            return "btn-primary";

        var today = AppDateTime.Today;
        if (today.Year == CurrentYear && today.Month == CurrentMonth && today.Day == day)
            return "btn-outline-dark";

        return "btn-outline-primary";
    }

    protected void OnYearSelected(int year)
    {
        CurrentYear = year;
        CurrentPart = CalendarPickerPart.Month;
    }

    protected void OnMonthSelected(int month)
    {
        CurrentMonth = month;
        CurrentPart = CalendarPickerPart.Day;
    }

    protected void OnDaySelected(int day)
    {
        var newValue = new DateTime(CurrentYear, CurrentMonth, day, 0, 0, 0, DateTimeKind.Utc);
        Value = newValue;
        ValueChanged?.Invoke(newValue);
    }

    protected void OnTodaySelected()
    {
        var today = AppDateTime.Today;
        CurrentYear = today.Year;
        CurrentMonth = today.Month;
        OnDaySelected(today.Day);
    }

    protected void PrevMonth()
    {
        if (CurrentMonth > 1)
            CurrentMonth--;
        else
        {
            CurrentYear--;
            CurrentMonth = 12;
        }
    }

    protected void NextMonth()
    {
        if (CurrentMonth < 12)
            CurrentMonth++;
        else
        {
            CurrentYear++;
            CurrentMonth = 1;
        }
    }
}

public enum CalendarPickerPart
{
    Year,
    Month,
    Day
}
