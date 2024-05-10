using Money.Models.Sorting;
using Money.Pages;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Queries;

/// <summary>
/// A query for getting a display type for balances.
/// </summary>
public class GetSummaryDisplayProperty : IQuery<SummaryDisplayType>
{ }
