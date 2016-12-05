using Money.Services.Models;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    /// <summary>
    /// A parameter class for <see cref="SummaryViewModel"/>.
    /// </summary>
    public class BarSummaryParameter : IGroupParameter
    {
        public MonthModel Month { get; set; }
        public YearModel Year { get; set; }
    }
}
