using System;
using Neptuo.Models.Keys;

namespace Money.Models;

public interface IExpenseOverviewModel
{
    IKey Key { get; }
    Price Amount { get; }
    DateTime When { get; }
    DateTime? ExpectedWhen { get; }
    IKey CategoryKey { get; }
    string Description { get; }
    bool IsFixed { get; }
}