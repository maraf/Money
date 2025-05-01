using Money.Models;
using Neptuo;
using Neptuo.Models.Keys;

namespace Money;

internal class MockData
{
    public static readonly OutcomeOverviewModel ExpenseOverviewModel = new OutcomeOverviewModel(
        KeyFactory.Create(typeof(Outcome)), 
        new Price(1, "USD"), 
        AppDateTime.Today, 
        string.Empty, 
        KeyFactory.Create(typeof(Category)), 
        false
    );

    public static readonly ExpenseTemplateCalendarMonthModel ExpenseTemplateCalendarMonthModel = new ExpenseTemplateCalendarMonthModel(
        1,
        1,
        KeyFactory.Create(typeof(ExpenseTemplate)), 
        new Price(1, "USD"), 
        0
    );

    public static readonly YearModel YearModel = new YearModel(
        2020
    );

    public static readonly MonthModel MonthModel = new MonthModel(
        2020,
        4
    );

    public static T Get<T>()
    {
        if (typeof(T) == typeof(OutcomeOverviewModel))
            return (T)(object)ExpenseOverviewModel;

        if (typeof(T) == typeof(ExpenseTemplateCalendarMonthModel))
            return (T)(object)ExpenseTemplateCalendarMonthModel;

        if (typeof(T) == typeof(YearModel))
            return (T)(object)YearModel;

        if (typeof(T) == typeof(MonthModel))
            return (T)(object)MonthModel;

        throw Ensure.Exception.NotSupported($"The type '{typeof(T).FullName}' is not supported.");
    }
}