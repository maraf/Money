using Money.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    /// <summary>
    /// A navigation class for pie chart summary page.
    /// </summary>
    public class PieSummaryParameter : IGroupParameter
    {
        public MonthModel Month { get; set; }
        public YearModel Year { get; set; }
    }
}
