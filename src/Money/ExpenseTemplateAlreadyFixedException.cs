using Neptuo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money;

/// <summary>
/// An exception raised when trying to set fixed on an expense template that is already in the same state.
/// </summary>
public class ExpenseTemplateAlreadyFixedException : AggregateRootException
{ }
