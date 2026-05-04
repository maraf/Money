using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Money.Components;

public partial class CalendarPicker(Interop Interop)
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

    protected ElementReference GridRef { get; set; }
    protected MonthView<int> MonthViewRef { get; set; }
    private bool needsGridNavSetup;

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

        needsGridNavSetup = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (needsGridNavSetup)
        {
            needsGridNavSetup = false;
            var gridElement = CurrentPart == CalendarPickerPart.Day && MonthViewRef != null
                ? MonthViewRef.BodyRef
                : GridRef;
            await Interop.SetupGridNavigationAsync(gridElement);
        }
    }

    protected bool IsSelectedDay(int day)
        => Value.Year == CurrentYear && Value.Month == CurrentMonth && Value.Day == day;

    protected string DayCssClass(int day)
    {
        if (IsSelectedDay(day))
            return "btn-primary";

        var today = AppDateTime.Today;
        if (today.Year == CurrentYear && today.Month == CurrentMonth && today.Day == day)
            return "btn-outline-dark";

        return "btn-outline-primary";
    }

    protected bool IsSelectedYear(int year) => Value.Year == year;

    protected bool IsSelectedMonth(int month) => Value.Year == CurrentYear && Value.Month == month;

    protected void OnYearSelected(int year)
    {
        CurrentYear = year;
        CurrentPart = CalendarPickerPart.Month;
        needsGridNavSetup = true;
    }

    protected void OnMonthSelected(int month)
    {
        CurrentMonth = month;
        CurrentPart = CalendarPickerPart.Day;
        needsGridNavSetup = true;
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
        needsGridNavSetup = true;
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
        needsGridNavSetup = true;
    }
}

public enum CalendarPickerPart
{
    Year,
    Month,
    Day
}
