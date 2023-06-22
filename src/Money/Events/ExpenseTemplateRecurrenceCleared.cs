using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events;

/// <summary>
/// An event raised when a recurrence of the expense template was removed.
/// </summary>
public class ExpenseTemplateRecurrenceCleared : UserEvent
{ }
