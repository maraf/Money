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
}