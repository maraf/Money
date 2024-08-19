using System;
using Neptuo;
using Neptuo.Events;

namespace Money.Events;

public class ExceptionRaised : Event
{
    public Exception Exception { get; }

    public ExceptionRaised(Exception exception)
    {
        Ensure.NotNull(exception, nameof(exception));
        Exception = exception;
    }
}