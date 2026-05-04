using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Linq;

namespace Money.Components;

public partial class MonthView
{
    [Parameter]
    public int Year { get; set; }

    [Parameter]
    public int Month { get; set; }

    [Parameter]
    public RenderFragment<int> ChildContent { get; set; }

    public ElementReference BodyRef { get; set; }

    protected string[] DayNames
    {
        get
        {
            var names = DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames;
            // Rotate so Monday is first
            return names.Skip(1).Concat(names.Take(1)).ToArray();
        }
    }

    protected int DaysInMonth => DateTime.DaysInMonth(Year, Month);

    protected int LeadingEmptyCells
    {
        get
        {
            var firstDay = new DateTime(Year, Month, 1).DayOfWeek;
            // Monday=0, Tuesday=1, ..., Sunday=6
            return ((int)firstDay + 6) % 7;
        }
    }
}
