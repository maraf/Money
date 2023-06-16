using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries;

/// <summary>
/// A query for getting list of months with expense or income.
/// </summary>
public class ListMonthWithExpenseOrIncome : UserQuery, IQuery<List<MonthModel>>
{ }
