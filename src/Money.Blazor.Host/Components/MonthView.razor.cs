using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;

namespace Money.Components;

public partial class MonthView<TItem>
{
    [Parameter]
    public int Year { get; set; }

    [Parameter]
    public int Month { get; set; }

    [Parameter]
    public RenderFragment<int> ChildContent { get; set; }

    protected string[] DayNames => DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames;

    protected int DaysInMonth => DateTime.DaysInMonth(Year, Month);

    protected int LeadingEmptyCells
    {
        get
        {
            var firstDay = new DateTime(Year, Month, 1).DayOfWeek;
            return (int)firstDay;
        }
    }
}
